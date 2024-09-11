using VLS.BatchExportNet.NWC;
using VLS.BatchExportNet.Transmit;
using VLS.BatchExportNet.Migrate;
using VLS.BatchExportNet.Detach;
using VLS.BatchExportNet.IFC;
using VLS.BatchExportNet.Link;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace VLS.BatchExportNet.Utils
{
    public class EventHandlerNWCExportBatchUiArg : RevitEventWrapper<NWCExportUi>
    {
        public override void Execute(UIApplication uiApp, NWCExportUi ui)
        {
            if (ui.ListBoxJsonConfigs.Items.Count == 0)
            {
                MessageBox.Show("Загрузите конфиги.");
                return;
            }

            DateTime timeStart = DateTime.Now;

            foreach (string config in ui.ListBoxJsonConfigs.Items)
            {
                try
                {
                    using FileStream file = File.OpenRead(config);
                    NWCForm form = JsonSerializer.Deserialize<NWCForm>(file);
                    ui.NWCFormDeserilaizer(form);
                }
                catch
                {
                    continue;
                }

                string folder = "";
                ui.Dispatcher.Invoke(() => folder = @ui.TextBoxFolder.Text);
                Logger logger = new Logger(folder);

                Methods.BatchExportNWC(uiApp, ui, ref logger);
                Thread.Sleep(3000);
            }

            TaskDialog taskDialog = new TaskDialog("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "ExportBatchNWCFinished",
                MainContent = $"Задание выполнено. Общее время выполнения: {DateTime.Now - timeStart}"
            };

            ui.IsEnabled = false;
            taskDialog.Show();
            ui.IsEnabled = true;
        }
    }

    public class EventHandlerNWCExportUiArg : RevitEventWrapper<NWCExportUi>
    {
        public override void Execute(UIApplication uiApp, NWCExportUi ui)
        {
            if (!UiExtMethods.IsEverythingFilled(ui))
            {
                return;
            }

            string folder = "";
            ui.Dispatcher.Invoke(() => folder = @ui.TextBoxFolder.Text);
            Logger logger = new(folder);

            Methods.BatchExportNWC(uiApp, ui, ref logger);

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "ExportNWCFinished",
                MainContent = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов."
            };

            ui.IsEnabled = false;
            taskDialog.Show();
            ui.IsEnabled = true;
        }
    }
    public class EventHandlerIFCExportUiArg : RevitEventWrapper<IFCExportUi>
    {
        public override void Execute(UIApplication uiApp, IFCExportUi ui)
        {
            if (!UiExtMethods.IsEverythingFilled(ui))
            {
                return;
            }

            string folder = "";
            ui.Dispatcher.Invoke(() => folder = @ui.TextBoxFolder.Text);
            Logger logger = new(folder);

            Methods.BatchExportIFC(uiApp, ui, ref logger);

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "ExportIFCFinished",
                MainContent = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов."
            };

            ui.IsEnabled = false;
            taskDialog.Show();
            ui.IsEnabled = true;
        }
    }

    public class EventHandlerDetachModelsUiArg : RevitEventWrapper<DetachModelsUi>
    {
        public override void Execute(UIApplication uiApp, DetachModelsUi ui)
        {
            if (!UiExtMethods.IsEverythingFilled(ui))
            {
                return;
            }

            Application application = uiApp.Application;
            List<ListBoxItem> listItems = @ui.listBoxItems.ToList();

            uiApp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(Methods.TaskDialogBoxShowingEvent);
            application.FailuresProcessing += new EventHandler<Autodesk.Revit.DB.Events.FailuresProcessingEventArgs>(Methods.Application_FailuresProcessing);
            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();

                if (!File.Exists(filePath))
                {
                    string error = $"Файла {filePath} не существует. Ты совсем Туттуру?";
                    item.Background = Brushes.Red;
                    continue;
                }


                Methods.DetachModel(application, filePath, ui);

            }
            uiApp.DialogBoxShowing -= new EventHandler<DialogBoxShowingEventArgs>(Methods.TaskDialogBoxShowingEvent);
            application.FailuresProcessing -= new EventHandler<Autodesk.Revit.DB.Events.FailuresProcessingEventArgs>(Methods.Application_FailuresProcessing);

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "DetachModelsFinished",
                MainContent = "Задание выполнено"
            };

            application.Dispose();

            ui.IsEnabled = false;
            taskDialog.Show();
            ui.IsEnabled = true;
        }
    }

    public class EventHandlerTransmitModelsUiArg : RevitEventWrapper<TransmitModelsUi>
    {
        public override void Execute(UIApplication uiApp, TransmitModelsUi ui)
        {
            if (!UiExtMethods.IsEverythingFilled(ui))
            {
                return;
            }

            Application application = uiApp.Application;
            List<ListBoxItem> listItems = @ui.listBoxItems.ToList();

            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();

                if (!File.Exists(filePath))
                {
                    string error = $"Файла {filePath} не существует. Ты совсем Туттуру?";
                    item.Background = Brushes.Red;
                    continue;
                }

                string folder = "";
                ui.Dispatcher.Invoke(() => folder = @ui.TextBoxFolder.Text);
                bool isSameFolder = (bool)ui.CheckBoxIsSameFolder.IsChecked;

                string transmittedFilePath = folder + "\\" + filePath.Split('\\').Last();
                File.Copy(filePath, transmittedFilePath, true);
                ModelPath transmittedModelPath = new FilePath(transmittedFilePath);
                Methods.UnloadRevitLinks(transmittedModelPath, isSameFolder, folder);
                //Methods.UnloadRevitLinks(new FilePath(filePath), isSameFolder, folder);
            }

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "TransmitModelsFinished",
                MainContent = "Задание выполнено"
            };

            application.Dispose();

            ui.IsEnabled = false;
            taskDialog.Show();
            ui.IsEnabled = true;
        }
    }

    public class EventHandlerMigrateModelsUiArg : RevitEventWrapper<MigrateModelsUi>
    {
        public override void Execute(UIApplication uiApp, MigrateModelsUi ui)
        {
            if (string.IsNullOrEmpty(ui.TextBoxConfig.Text)
                || !ui.TextBoxConfig.Text.EndsWith(".json", true, CultureInfo.InvariantCulture))
            {
                MessageBox.Show("Предоставьте ссылку на конфиг");
                return;
            }

            Dictionary<string, string> items;
            List<string> movedFiles = new List<string>();
            List<string> failedFiles = new List<string>();

            using (FileStream file = File.OpenRead(ui.TextBoxConfig.Text))
            {
                try
                {
                    items = JsonSerializer.Deserialize<Dictionary<string, string>>(file);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Неверная схема файла");
                    return;
                }
            }

            Application application = uiApp.Application;

            foreach (string oldFile in items.Keys)
            {
                if (!File.Exists(oldFile))
                {
                    failedFiles.Add(oldFile);
                    continue;
                }

                string newFile = items.FirstOrDefault(e => e.Key == oldFile).Value;

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFile));
                    File.Copy(oldFile, newFile, true);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                    failedFiles.Add(oldFile);
                    continue;
                }

                movedFiles.Add(newFile);
            }

            foreach (string newFile in movedFiles)
            {
                ModelPath newFilePath = new FilePath(newFile);
                Methods.ReplaceRevitLinks(newFilePath, items);

                Document document = OpenDocument.OpenTransmitted(application, newFilePath);

                newFilePath.Dispose();

                try
                {
                    Methods.FreeTheModel(document);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                document.Close();
                document.Dispose();
            }

            TaskDialog taskDialog = new TaskDialog("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "MigrateModelsFinished",
                MainContent = $"Задание выполнено.\nСледующие файлы не были скопированы:\n{string.Join("\n", failedFiles)}"
            };

            application.Dispose();

            ui.IsEnabled = false;
            taskDialog.Show();
            ui.IsEnabled = true;
        }
    }
    public class EventHandlerLinkModelsUiArg : RevitEventWrapper<LinkModelsUi>
    {
        public override void Execute(UIApplication uiApp, LinkModelsUi ui)
        {
            Methods.LinkRevitModel(uiApp, ui);

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "LinkModelsFinished",
                MainContent = "Задание выполнено"
            };

            ui.IsEnabled = false;
            taskDialog.Show();
            ui.IsEnabled = true;
        }
    }
}