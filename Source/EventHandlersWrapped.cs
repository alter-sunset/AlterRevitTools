using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text.Json;
using System.Globalization;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.NWC;
using VLS.BatchExportNet.Views.IFC;
using VLS.BatchExportNet.Views.Link;
using VLS.BatchExportNet.Views.Detach;
using VLS.BatchExportNet.Views.Migrate;
using VLS.BatchExportNet.Views.Transmit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace VLS.BatchExportNet.Source
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
                Logger logger = new(folder);

                NWCHelper.BatchExportModels(uiApp, ui, ref logger);
                logger.Dispose();
                Thread.Sleep(3000);
            }

            TaskDialog taskDialog = new("Готово!")
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
            if (!ViewHelper.IsEverythingFilled(ui))
            {
                return;
            }

            string folder = "";
            ui.Dispatcher.Invoke(() => folder = @ui.TextBoxFolder.Text);
            Logger logger = new(folder);

            NWCHelper.BatchExportModels(uiApp, ui, ref logger);

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "ExportNWCFinished",
                MainContent = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов."
            };

            logger.Dispose();
            ui.IsEnabled = false;
            taskDialog.Show();
            ui.IsEnabled = true;
        }
    }

    public class EventHandlerIFCExportUiArg : RevitEventWrapper<IFCExportUi>
    {
        public override void Execute(UIApplication uiApp, IFCExportUi ui)
        {
            if (!ViewHelper.IsEverythingFilled(ui))
            {
                return;
            }

            string folder = "";
            ui.Dispatcher.Invoke(() => folder = @ui.TextBoxFolder.Text);
            Logger logger = new(folder);

            IFCHelper.BatchExportModels(uiApp, ui, ref logger);

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "ExportIFCFinished",
                MainContent = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов."
            };

            logger.Dispose();
            ui.IsEnabled = false;
            taskDialog.Show();
            ui.IsEnabled = true;
        }
    }

    public class EventHandlerDetachModelsUiArg : RevitEventWrapper<DetachModelsUi>
    {
        public override void Execute(UIApplication uiApp, DetachModelsUi ui)
        {
            if (!ViewHelper.IsEverythingFilled(ui))
            {
                return;
            }

            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = [.. @ui.listBoxItems];

            uiApp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(ErrorSwallowersHelper.TaskDialogBoxShowingEvent);
            application.FailuresProcessing += new EventHandler<Autodesk.Revit.DB.Events.FailuresProcessingEventArgs>(ErrorSwallowersHelper.Application_FailuresProcessing);
            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();

                if (!File.Exists(filePath))
                {
                    string error = $"Файла {filePath} не существует. Ты совсем Туттуру?";
                    item.Background = Brushes.Red;
                    continue;
                }

                DetachModel(application, filePath, ui);
            }
            uiApp.DialogBoxShowing -= new EventHandler<DialogBoxShowingEventArgs>(ErrorSwallowersHelper.TaskDialogBoxShowingEvent);
            application.FailuresProcessing -= new EventHandler<Autodesk.Revit.DB.Events.FailuresProcessingEventArgs>(ErrorSwallowersHelper.Application_FailuresProcessing);

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "DetachModelsFinished",
                MainContent = "Задание выполнено"
            };

            ui.IsEnabled = false;
            taskDialog.Show();
            ui.IsEnabled = true;
        }
        private static void DetachModel(Application application, string filePath, DetachModelsUi ui)
        {
            Document document;
            BasicFileInfo fileInfo;
            bool isWorkshared = true;
            try
            {
                fileInfo = BasicFileInfo.Extract(filePath);
                if (!fileInfo.IsWorkshared)
                {
                    document = application.OpenDocumentFile(filePath);
                    isWorkshared = false;
                }
                else
                {
                    ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                    WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.CloseAllWorksets);
                    document = OpenDocumentHelper.OpenDetached(application, modelPath, worksetConfiguration);
                    isWorkshared = true;
                }
            }
            catch
            {
                return;
            }
            //RevitLinksHelper.Delete(document);
            ModelHelper.DeleteAllLinks(document); //Delete all links instead of just rvt links
            string fileDetachedPath = "";
            switch (ui.RadioButtonSavingPathMode)
            {
                case 1:
                    string folder = "";
                    ui.Dispatcher.Invoke(() => folder = @ui.TextBoxFolder.Text);
                    fileDetachedPath = folder + "\\" + document.Title.Replace("_detached", "").Replace("_отсоединено", "") + ".rvt";
                    break;
                case 3:
                    string maskIn = "";
                    string maskOut = "";
                    ui.Dispatcher.Invoke(() => maskIn = @ui.TextBoxMaskIn.Text);
                    ui.Dispatcher.Invoke(() => maskOut = @ui.TextBoxMaskOut.Text);
                    fileDetachedPath = @filePath.Replace(maskIn, maskOut);
                    break;
            }

            SaveAsOptions saveAsOptions = new()
            {
                OverwriteExistingFile = true,
                MaximumBackups = 1
            };
            WorksharingSaveAsOptions worksharingSaveAsOptions = new()
            {
                SaveAsCentral = true
            };
            if (isWorkshared)
                saveAsOptions.SetWorksharingOptions(worksharingSaveAsOptions);

            ModelPath modelDetachedPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileDetachedPath);
            document?.SaveAs(modelDetachedPath, saveAsOptions);

            try
            {
                if (isWorkshared)
                    ModelHelper.FreeTheModel(document);
            }
            catch
            {
            }

            document?.Close();
            document?.Dispose();
        }
    }

    public class EventHandlerTransmitModelsUiArg : RevitEventWrapper<TransmitModelsUi>
    {
        public override void Execute(UIApplication uiApp, TransmitModelsUi ui)
        {
            if (!ViewHelper.IsEverythingFilled(ui))
            {
                return;
            }

            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = [.. @ui.listBoxItems];

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
                ModelPath transmittedModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(transmittedFilePath);
                RevitLinksHelper.Unload(transmittedModelPath, isSameFolder, folder);
            }

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "TransmitModelsFinished",
                MainContent = "Задание выполнено"
            };

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
            List<string> movedFiles = [];
            List<string> failedFiles = [];

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

            using Application application = uiApp.Application;

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
                ModelPath newFilePath = ModelPathUtils.ConvertUserVisiblePathToModelPath(newFile);
                RevitLinksHelper.Replace(newFilePath, items);

                using Document document = OpenDocumentHelper.OpenTransmitted(application, newFilePath);

                try
                {
                    ModelHelper.FreeTheModel(document);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                document.Close();
            }

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "MigrateModelsFinished",
                MainContent = $"Задание выполнено.\nСледующие файлы не были скопированы:\n{string.Join("\n", failedFiles)}"
            };

            ui.IsEnabled = false;
            taskDialog.Show();
            ui.IsEnabled = true;
        }
    }

    public class EventHandlerLinkModelsUiArg : RevitEventWrapper<LinkViewModel>
    {
        public override void Execute(UIApplication uiApp, LinkViewModel linkViewModel)
        {
            RevitLinksHelper.CreateLinks(uiApp, linkViewModel);

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "LinkModelsFinished",
                MainContent = "Задание выполнено"
            };

            linkViewModel.IsViewEnabled = false;
            taskDialog.Show();
            linkViewModel.IsViewEnabled = true;
        }
    }
}