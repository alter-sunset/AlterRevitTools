using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Link;
using AlterTools.BatchExport.Views.Detach;
using AlterTools.BatchExport.Views.Transmit;
using AlterTools.BatchExport.Views.Params;

namespace AlterTools.BatchExport.Utils
{
    static class ViewModelHelper
    {
        private const string NO_FOLDER = "Укажите папку для экспорта!";
        private const string WRONG_FOLDER = "Укажите корректную папку для экспорта!";
        private const string CREATE_FOLDER = "Такой папки не существует.\nСоздать папку?";
        private const string TO_HELL = "Нет, так нет.\nТогда живи в проклятом мире, который сам и создал.";
        private const string NO_FILES = "Добавьте хотя бы один файл для экспорта!";
        private const string NO_VIEW_NAME = "Введите имя вида!";
        private const string NO_PATH_MODE = "Выберите режим выбора пути!";
        private const string NO_MASK_PATH = "Укажите маску замены пути!";
        private const string WRONG_MASK = "Несоответствие входной маски и имён файлов!";
        private const string NO_MASK_FILE = "Введите маску для переименования файлов!";
        private const string NO_CSV = "Укажите корректный путь к выходному файлу!";
        private const string NO_PARAMETERS = "Укажите хотя бы один параметр для экспорта!";

        internal static bool IsEverythingFilled(this DetachViewModel detachVM)
        {
            return detachVM.IsListNotEmpty()
                && detachVM.IsRBModeOK()
                && detachVM.IsViewNameOK()
                && detachVM.IsMaskNameOK();
        }

        internal static bool IsEverythingFilled(this TransmitViewModel transmitVM)
        {
            return transmitVM.IsListNotEmpty()
                && transmitVM.IsFolderPathOK();
        }

        internal static bool IsEverythingFilled(this ParamsViewModel paramsVM)
        {
            return paramsVM.IsListNotEmpty()
                && paramsVM.IsCsvPathNotEmpty()
                && paramsVM.AreThereAnyParameters();
        }

        internal static bool IsEverythingFilled(this ViewModelBase_Extended vmBaseExt)
        {
            return vmBaseExt.IsListNotEmpty()
                && vmBaseExt.IsFolderPathOK()
                && vmBaseExt.IsViewNameOK();
        }

        internal static bool IsEverythingFilled(this LinkViewModel linkVM) => linkVM.IsListNotEmpty();

        private static bool IsListNotEmpty(this LinkViewModel linkVM) => CheckCondition(linkVM.Entries.Count > 0, NO_FILES);
        private static bool IsListNotEmpty(this ViewModelBase vmBase) => CheckCondition(vmBase.ListBoxItems.Count > 0, NO_FILES);

        private static bool IsFolderPathOK(this ViewModelBase vmBase)
        {
            string folderPath = vmBase.FolderPath;

            if (string.IsNullOrEmpty(folderPath)) return CheckCondition(false, NO_FOLDER);

            if (Uri.IsWellFormedUriString(folderPath, UriKind.RelativeOrAbsolute)) return CheckCondition(false, WRONG_FOLDER);

            if (!Directory.Exists(folderPath))
            {
                MessageBoxResult result = MessageBox.Show(CREATE_FOLDER, "Добрый вечер", MessageBoxButton.YesNo);

                if (result is MessageBoxResult.Yes)
                {
                    Directory.CreateDirectory(folderPath);
                }
                else
                {
                    MessageBox.Show(TO_HELL);
                    return false;
                }
            }

            return true;
        }

        private static bool IsViewNameOK(this ViewModelBase_Extended vmBaseExt)
        {
            return CheckCondition(!vmBaseExt.ExportScopeView
                                  || !string.IsNullOrEmpty(vmBaseExt.ViewName), NO_VIEW_NAME);
        }
        private static bool IsViewNameOK(this DetachViewModel detachVM)
        {
            return CheckCondition(!detachVM.CheckForEmptyView
                                  || !string.IsNullOrEmpty(detachVM.ViewName), NO_VIEW_NAME);
        }

        private static bool IsRBModeOK(this DetachViewModel detachVM)
        {
            switch (detachVM.RadioButtonMode)
            {
                case 0:
                    return CheckCondition(false, NO_PATH_MODE);

                case 1:
                    return detachVM.IsFolderPathOK();

                case 2:
                    if (string.IsNullOrEmpty(detachVM.MaskIn) || string.IsNullOrEmpty(detachVM.MaskOut))
                    {
                        return CheckCondition(false, NO_MASK_PATH);
                    }

                    if (!detachVM.ListBoxItems.Select(e => e.Content)
                                              .All(e => e.ToString().Contains(detachVM.MaskIn)))
                    {
                        return CheckCondition(false, WRONG_MASK);
                    }

                    break;
            }

            return true;
        }

        private static bool IsMaskNameOK(this DetachViewModel detachVM)
        {
            return CheckCondition(!detachVM.IsToRename
                                  || !string.IsNullOrEmpty(detachVM.MaskInName), NO_MASK_FILE);
        }

        private static bool IsCsvPathNotEmpty(this ParamsViewModel paramsVM)
        {
            string csvPath = paramsVM.CsvPath;

            if (string.IsNullOrWhiteSpace(csvPath)
                || Uri.IsWellFormedUriString(csvPath, UriKind.Absolute)
                || !csvPath.EndsWith(".csv"))
            {
                return CheckCondition(false, NO_CSV);
            }

            return true;

        }
        private static bool AreThereAnyParameters(this ParamsViewModel paramsVM) => CheckCondition(paramsVM.ParametersNames.Length > 0, NO_PARAMETERS);

        private static bool CheckCondition(bool condition, string msg)
        {
            if (!condition) MessageBox.Show(msg);

            return condition;
        }

        /// <summary>
        /// Default finisher method of most plugins,
        /// that will show final TaskDialog and lock View until TaskDialog is closed
        /// </summary>
        /// <param name="id">TaskDialog Id</param>
        /// <param name="msg">Message to show to user</param>
        public static void Finisher(this ViewModelBase vmBase, string id, string msg = "Задание выполнено")
        {
            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = id,
                MainContent = msg
            };

            vmBase.IsViewEnabled = false;
            taskDialog.Show();
            vmBase.IsViewEnabled = true;
            //TODO: Rework this 
            //Process.Start("shutdown", "/s /t 10");
        }

        /// <returns>Unique files with .rvt extension</returns>
        public static IEnumerable<string> FilterRevitFiles(this IEnumerable<string> files)
        {
            return files.Distinct()
                        .Where(f => !string.IsNullOrWhiteSpace(f)
                                    && Path.GetExtension(f) == ".rvt");
        }

        public static string RemoveDetach(this string name)
        {
            return name.Replace("_detached", "")
                       .Replace("_отсоединено", "");
        }

        public static string[] SplitBySemicolon(this string line)
        {
            return line.Split(';')
                       .Select(e => e.Trim())
                       .Distinct()
                       .Where(e => !string.IsNullOrWhiteSpace(e))
                       .ToArray();
        }
    }
}