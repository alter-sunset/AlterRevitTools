using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using VLS.BatchExportNet.Views.Base;
using VLS.BatchExportNet.Views.Detach;
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
        internal static bool IsEverythingFilled(this ViewModelBase viewModel)
        {
            return viewModel.IsListNotEmpty();
        }

        private static bool IsListNotEmpty(this ViewModelBase viewModel)
        {
            if (viewModel.ListBoxItems.Count == 0)
            {
                MessageBox.Show(AlertType.NoFiles.GetAlert());
                return false;
            }
            return true;
        }
        private static bool IsFolderPathOK(this ViewModelBase viewModel)
        {
            string textBoxFolder = viewModel.FolderPath;
            if (string.IsNullOrEmpty(textBoxFolder))
            {
                MessageBox.Show(AlertType.NoFolder.GetAlert());
                return false;
            }
            if (Uri.IsWellFormedUriString(textBoxFolder, UriKind.RelativeOrAbsolute))
            {
                MessageBox.Show(AlertType.IncorrectFolder.GetAlert());
                return false;
            }
            if (!Directory.Exists(textBoxFolder))
            {
                MessageBoxResult messageBox = MessageBox.Show(AlertType.FolderNotExist.GetAlert(),
                    "Добрый вечер", MessageBoxButton.YesNo);
                switch (messageBox)
                {
                    case MessageBoxResult.Yes:
                        Directory.CreateDirectory(textBoxFolder);
                        break;
                    case MessageBoxResult.No:
                    case MessageBoxResult.Cancel:
                        MessageBox.Show(AlertType.ToHell.GetAlert());
                        return false;
                }
            }
            return true;
        }
        private static bool IsViewNameOK(this ViewModelBase_Extended viewModel)
        {
            if (viewModel.ExportScopeView && string.IsNullOrEmpty(viewModel.ViewName))
            {
                MessageBox.Show(AlertType.NoViewNameToExport.GetAlert());
                return false;
            }
            return true;
        }
        private static bool IsViewNameOK(this DetachViewModel viewModel)
        {
            if (viewModel.CheckForEmpty && string.IsNullOrEmpty(viewModel.ViewName))
            {
                MessageBox.Show(AlertType.NoViewNameToCheck.GetAlert());
                return false;
            }
            return true;
        }
        private static bool IsRBModeOK(this DetachViewModel detachViewModel)
        {
            switch (detachViewModel.RadioButtonMode)
            {
                case 0:
                    MessageBox.Show(AlertType.NoPathMode.GetAlert());
                    return false;
                case 1:
                    return IsFolderPathOK(detachViewModel);
                case 2:
                    if (string.IsNullOrEmpty(detachViewModel.MaskIn) || string.IsNullOrEmpty(detachViewModel.MaskOut))
                    {
                        MessageBox.Show(AlertType.NoMaskFolder.GetAlert());
                        return false;
                    }
                    if (!detachViewModel.ListBoxItems.Select(e => e.Content)
                        .All(e => e.ToString().Contains(detachViewModel.MaskIn)))
                    {
                        MessageBox.Show(AlertType.WrongMask.GetAlert());
                        return false;
                    }
                    break;
            }
            return true;
        }
        private static bool IsMaskNameOK(this DetachViewModel detachViewModel)
        {
            if (detachViewModel.IsToRename && string.IsNullOrEmpty(detachViewModel.MaskInName))
            {
                MessageBox.Show(AlertType.NoMaskFile.GetAlert());
                return false;
            }
            return true;
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