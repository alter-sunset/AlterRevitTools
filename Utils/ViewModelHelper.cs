using System;
using System.IO;
using System.Linq;
using System.Windows;
using VLS.BatchExportNet.Views;
using VLS.BatchExportNet.Views.Detach;
using VLS.BatchExportNet.Views.Transmit;

namespace VLS.BatchExportNet.Utils
{
    static class ViewModelHelper
    {
        internal static bool IsEverythingFilled(DetachViewModel detachViewModel)
        {
            return IsListNotEmpty(detachViewModel) && IsRBModeOK(detachViewModel);
        }
        internal static bool IsEverythingFilled(TransmitViewModel viewModel)
        {
            return IsListNotEmpty(viewModel) && IsFolderPathOK(viewModel);
        }
        internal static bool IsEverythingFilled(ViewModelBase_Extended viewModel)
        {
            return IsListNotEmpty(viewModel) && IsFolderPathOK(viewModel) && IsViewNameOK(viewModel);
        }

        private static bool IsListNotEmpty(ViewModelBase viewModel)
        {
            if (viewModel.ListBoxItems.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один файл для экспорта!");
                return false;
            }
            return true;
        }
        private static bool IsFolderPathOK(ViewModelBase viewModel)
        {
            string textBoxFolder = viewModel.FolderPath;
            if (string.IsNullOrEmpty(textBoxFolder))
            {
                MessageBox.Show("Укажите папку для экспорта!");
                return false;
            }
            if (Uri.IsWellFormedUriString(textBoxFolder, UriKind.RelativeOrAbsolute))
            {
                MessageBox.Show("Укажите корректную папку для экспорта!");
                return false;
            }
            if (!Directory.Exists(textBoxFolder))
            {
                MessageBoxResult messageBox = MessageBox
                    .Show("Такой папки не существует.\nСоздать папку?", "Добрый вечер", MessageBoxButton.YesNo);
                switch (messageBox)
                {
                    case MessageBoxResult.Yes:
                        Directory.CreateDirectory(textBoxFolder);
                        break;
                    case MessageBoxResult.No:
                    case MessageBoxResult.Cancel:
                        MessageBox.Show("Нет, так нет.\nТогда живи в проклятом мире, который сам и создал.");
                        return false;
                }
            }
            return true;
        }
        private static bool IsViewNameOK(ViewModelBase_Extended viewModel)
        {
            if (viewModel.ExportScopeView && string.IsNullOrEmpty(viewModel.ViewName))
            {
                MessageBox.Show("Введите имя вида для экспорта!");
                return false;
            }
            return true;
        }
        private static bool IsRBModeOK(DetachViewModel detachViewModel)
        {
            switch (detachViewModel.RadionButtonMode)
            {
                case 0:
                    MessageBox.Show("Выберите режим выбора пути");
                    return false;
                case 1:
                    return IsFolderPathOK(detachViewModel);
                case 2:
                    if (string.IsNullOrEmpty(detachViewModel.MaskIn) || string.IsNullOrEmpty(detachViewModel.MaskOut))
                    {
                        MessageBox.Show("Укажите маску замены");
                        return false;
                    }
                    if (!detachViewModel.ListBoxItems.Select(e => e.Content)
                        .All(e => e.ToString().Contains(detachViewModel.MaskIn)))
                    {
                        MessageBox.Show("Несоответсвие входной маски и имён файлов.");
                        return false;
                    }
                    break;
            }
            return true;
        }
    }
}