using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using VLS.BatchExportNet.Views.Base;
using VLS.BatchExportNet.Views.Detach;
using VLS.BatchExportNet.Views.Link;
using VLS.BatchExportNet.Views.Transmit;

namespace VLS.BatchExportNet.Utils
{
    static class ViewModelHelper
    {
        internal static bool IsEverythingFilled(this DetachViewModel detachViewModel)
        {
            return detachViewModel.IsListNotEmpty()
                && detachViewModel.IsRBModeOK()
                && detachViewModel.IsViewNameOK()
                && detachViewModel.IsMaskNameOK();
        }
        internal static bool IsEverythingFilled(this TransmitViewModel viewModel)
        {
            return viewModel.IsListNotEmpty()
                && viewModel.IsFolderPathOK();
        }
        internal static bool IsEverythingFilled(this ViewModelBase_Extended viewModel)
        {
            return viewModel.IsListNotEmpty()
                && viewModel.IsFolderPathOK()
                && viewModel.IsViewNameOK();
        }
        internal static bool IsEverythingFilled(this LinkViewModel viewModel)
        {
            return viewModel.IsListNotEmpty();
        }

        private static bool IsListNotEmpty(this LinkViewModel viewModel) =>
            CheckCondition(viewModel.Entries.Count > 0, "Добавьте хотя бы один файл для экспорта!");
        private static bool IsListNotEmpty(this ViewModelBase viewModel) =>
            CheckCondition(viewModel.ListBoxItems.Count > 0, "Добавьте хотя бы один файл для экспорта!");
        private static bool IsFolderPathOK(this ViewModelBase viewModel)
        {
            string folderPath = viewModel.FolderPath;

            if (string.IsNullOrEmpty(folderPath))
                return CheckCondition(false, "Укажите папку для экспорта!");

            if (Uri.IsWellFormedUriString(folderPath, UriKind.RelativeOrAbsolute))
                return CheckCondition(false, "Укажите корректную папку для экспорта!");

            if (!Directory.Exists(folderPath))
            {
                MessageBoxResult result = MessageBox.Show("Такой папки не существует.\nСоздать папку?",
                    "Добрый вечер", MessageBoxButton.YesNo);
                if (result is MessageBoxResult.Yes) Directory.CreateDirectory(folderPath);

                else
                {
                    MessageBox.Show("Нет, так нет.\nТогда живи в проклятом мире, который сам и создал.");
                    return false;
                }
            }
            return true;
        }
        private static bool IsViewNameOK(this ViewModelBase_Extended viewModel) =>
            CheckCondition(!viewModel.ExportScopeView || !string.IsNullOrEmpty(viewModel.ViewName),
                "Введите имя вида для экспорта!");
        private static bool IsViewNameOK(this DetachViewModel viewModel) =>
            CheckCondition(!viewModel.CheckForEmptyView || !string.IsNullOrEmpty(viewModel.ViewName),
                "Введите имя вида для проверки!");
        private static bool IsRBModeOK(this DetachViewModel detachViewModel)
        {
            switch (detachViewModel.RadioButtonMode)
            {
                case 0:
                    return CheckCondition(false, "Выберите режим выбора пути!");
                case 1:
                    return detachViewModel.IsFolderPathOK();
                case 2:
                    if (string.IsNullOrEmpty(detachViewModel.MaskIn) || string.IsNullOrEmpty(detachViewModel.MaskOut))
                        return CheckCondition(false, "Укажите маску замены пути!");

                    if (!detachViewModel.ListBoxItems.Select(e => e.Content)
                        .All(e => e.ToString().Contains(detachViewModel.MaskIn)))
                        return CheckCondition(false, "Несоответствие входной маски и имён файлов!");
                    break;
            }
            return true;
        }
        private static bool IsMaskNameOK(this DetachViewModel detachViewModel) =>
             CheckCondition(!detachViewModel.IsToRename || !string.IsNullOrEmpty(detachViewModel.MaskInName),
                "Введите маски для переименования файлов!");
        private static bool CheckCondition(bool condition, string message)
        {
            if (!condition) MessageBox.Show(message);
            return condition;
        }

        /// <summary>
        /// Default finisher method of most plugins,
        /// that will show final TaskDialog and lock View until TaskDialog is closed
        /// </summary>
        /// <param name="id">TaskDialog Id</param>
        /// <param name="msg">Message to show to user</param>
        public static void Finisher(this ViewModelBase viewModel, string id, string msg = "Задание выполнено")
        {
            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = id,
                MainContent = msg
            };
            viewModel.IsViewEnabled = false;
            taskDialog.Show();
            viewModel.IsViewEnabled = true;
        }
    }
}