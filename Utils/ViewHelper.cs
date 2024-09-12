using System;
using System.IO;
using System.Linq;
using System.Windows;
using VLS.BatchExportNet.Source;
using VLS.BatchExportNet.Views.IFC;
using VLS.BatchExportNet.Views.NWC;
using VLS.BatchExportNet.Views.Link;
using VLS.BatchExportNet.Views.Detach;
using VLS.BatchExportNet.Views.Migrate;
using VLS.BatchExportNet.Views.Transmit;
using Autodesk.Revit.UI;

namespace VLS.BatchExportNet.Utils
{
    static class ViewHelper
    {
        private static Window _myForm;
        internal static void ShowForm(UIApplication uiapp, Forms form)
        {
            if (_myForm != null && _myForm == null) return;

            try
            {
                switch (form)
                {
                    case Forms.Detach:
                        EventHandlerDetachModelsUiArg evDetachUi = new();
                        _myForm = new DetachModelsUi(uiapp, evDetachUi) { Height = 600, Width = 800 };
                        break;
                    case Forms.IFC:
                        EventHandlerIFCExportUiArg evIFCUi = new();
                        _myForm = new IFCExportUi(uiapp, evIFCUi) { Height = 700, Width = 800 };
                        break;
                    case Forms.NWC:
                        EventHandlerNWCExportUiArg evNWCUi = new();
                        EventHandlerNWCExportBatchUiArg eventHandlerNWCExportBatchUiArg = new();
                        _myForm = new NWCExportUi(uiapp, evNWCUi, eventHandlerNWCExportBatchUiArg) { Height = 900, Width = 800 };
                        break;
                    case Forms.Migrate:
                        EventHandlerMigrateModelsUiArg evMigrateUi = new();
                        _myForm = new MigrateModelsUi(uiapp, evMigrateUi) { Height = 200, Width = 600 };
                        break;
                    case Forms.Transmit:
                        EventHandlerTransmitModelsUiArg evTransmitUi = new();
                        _myForm = new TransmitModelsUi(uiapp, evTransmitUi) { Height = 500, Width = 800 };
                        break;
                    case Forms.Link:
                        EventHandlerLinkModelsUiArg evLinkUi = new();
                        _myForm = new LinkModelsUi(uiapp, evLinkUi) { Height = 500, Width = 800 };
                        break;
                }
                _myForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        internal static bool IsEverythingFilled(NWCExportUi ui)
        {
            if (ui.listBoxItems.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один файл для экспорта!");
                return false;
            }

            string textBoxFolder = "";
            ui.Dispatcher.Invoke(() => textBoxFolder = ui.TextBoxFolder.Text);

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

            string viewName = "";
            ui.Dispatcher.Invoke(() => viewName = ui.TextBoxExportScopeViewName.Text);

            if ((bool)ui.RadioButtonExportScopeView.IsChecked && string.IsNullOrEmpty(viewName))
            {
                MessageBox.Show("Введите имя вида для экспорта!");
                return false;
            }

            if (!Directory.Exists(textBoxFolder))
            {
                bool isIt = true;

                MessageBoxResult messageBox = MessageBox
                    .Show("Такой папки не существует.\nСоздать папку?", "Добрый вечер", MessageBoxButton.YesNo);
                switch (messageBox)
                {
                    case MessageBoxResult.Yes:
                        Directory.CreateDirectory(textBoxFolder);
                        break;
                    case MessageBoxResult.No:
                    case MessageBoxResult.Cancel:
                        isIt = false;
                        MessageBox.Show("Нет, так нет.\nТогда живи в проклятом мире, который сам и создал.");
                        break;
                }
                if (!isIt)
                {
                    return isIt;
                }
            }
            return true;
        }
        internal static bool IsEverythingFilled(IFCExportUi ui)
        {
            if (ui.listBoxItems.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один файл для экспорта!");
                return false;
            }

            string textBoxFolder = "";
            ui.Dispatcher.Invoke(() => textBoxFolder = ui.TextBoxFolder.Text);

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

            string viewName = "";
            ui.Dispatcher.Invoke(() => viewName = ui.TextBoxExportScopeViewName.Text);

            if ((bool)ui.RadioButtonExportScopeView.IsChecked && string.IsNullOrEmpty(viewName))
            {
                MessageBox.Show("Введите имя вида для экспорта!");
                return false;
            }

            if (!Directory.Exists(textBoxFolder))
            {
                bool isIt = true;

                MessageBoxResult messageBox = MessageBox
                    .Show("Такой папки не существует.\nСоздать папку?", "Добрый вечер", MessageBoxButton.YesNo);
                switch (messageBox)
                {
                    case MessageBoxResult.Yes:
                        Directory.CreateDirectory(textBoxFolder);
                        break;
                    case MessageBoxResult.No:
                    case MessageBoxResult.Cancel:
                        isIt = false;
                        MessageBox.Show("Нет, так нет.\nТогда живи в проклятом мире, который сам и создал.");
                        break;
                }
                if (!isIt)
                {
                    return isIt;
                }
            }
            return true;
        }
        internal static bool IsEverythingFilled(DetachModelsUi ui)
        {
            if (ui.listBoxItems.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один файл для экспорта!");
                return false;
            }

            switch (ui.RadioButtonSavingPathMode)
            {
                case 0:
                    MessageBox.Show("Выберите режим выбора пути");
                    return false;
                case 1:
                    string textBoxFolder = "";
                    ui.Dispatcher.Invoke(() => textBoxFolder = ui.TextBoxFolder.Text);

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
                    break;
                case 3:
                    string maskIn = "";
                    string maskOut = "";
                    ui.Dispatcher.Invoke(() => maskIn = ui.TextBoxMaskIn.Text);
                    ui.Dispatcher.Invoke(() => maskOut = ui.TextBoxMaskOut.Text);

                    if (string.IsNullOrEmpty(maskIn) || string.IsNullOrEmpty(maskOut))
                    {
                        MessageBox.Show("Укажите маску замены");
                        return false;
                    }
                    if (!ui.listBoxItems.Select(e => e.Content)
                        .All(e => e.ToString().Contains(maskIn)))
                    {
                        MessageBox.Show("Несоответсвие входной маски и имён файлов.");
                        return false;
                    }
                    break;
            }
            return true;
        }
        internal static bool IsEverythingFilled(TransmitModelsUi ui)
        {
            if (!ui.listBoxItems.Any())
            {
                MessageBox.Show("Добавьте хотя бы один файл для экспорта!");
                return false;
            }

            string textBoxFolder = "";
            ui.Dispatcher.Invoke(() => textBoxFolder = ui.TextBoxFolder.Text);

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
                bool isIt = true;
                const string ABORT = "Нет, так нет.\nТогда живи в проклятом мире, который сам и создал.";

                MessageBoxResult messageBox = MessageBox
                    .Show("Такой папки не существует.\nСоздать папку?", "Добрый вечер", MessageBoxButton.YesNo);
                switch (messageBox)
                {
                    case MessageBoxResult.Yes:
                        Directory.CreateDirectory(textBoxFolder);
                        break;
                    case MessageBoxResult.No:
                        isIt = false;
                        MessageBox.Show(ABORT);
                        break;
                    case MessageBoxResult.Cancel:
                        isIt = false;
                        MessageBox.Show(ABORT);
                        break;
                }
                if (!isIt)
                {
                    return isIt;
                }
            }
            return true;
        }
    }
}