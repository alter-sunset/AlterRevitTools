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
using Autodesk.Revit.DB.Events;

namespace VLS.BatchExportNet.Source
{
    public class EventHandlerNWCExportBatchUiArg : RevitEventWrapper<NWC_ViewModel>
    {
        public override void Execute(UIApplication uiApp, NWC_ViewModel nwc_ViewModel)
        {
            if (nwc_ViewModel.Configs.Count == 0)
            {
                MessageBox.Show("Загрузите конфиги.");
                return;
            }

            DateTime timeStart = DateTime.Now;

            foreach (string config in nwc_ViewModel.Configs)
            {
                try
                {
                    using FileStream file = File.OpenRead(config);
                    NWCForm form = JsonSerializer.Deserialize<NWCForm>(file);
                    nwc_ViewModel.NWCFormDeserilaizer(form);
                }
                catch
                {
                    continue;
                }

                string folder = nwc_ViewModel.FolderPath;
                Logger logger = new(folder);

                NWCHelper.BatchExportModels(uiApp, nwc_ViewModel, ref logger);
                logger.Dispose();
                Thread.Sleep(1000);
            }

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "ExportBatchNWCFinished",
                MainContent = $"Задание выполнено. Всего затрачено времени:{DateTime.Now - timeStart}"
            };
            nwc_ViewModel.IsViewEnabled = false;
            taskDialog.Show();
            nwc_ViewModel.IsViewEnabled = true;
        }
    }

    public class EventHandlerNWCExportUiArg : RevitEventWrapper<NWC_ViewModel>
    {
        public override void Execute(UIApplication uiApp, NWC_ViewModel nwc_ViewModel)
        {
            if (!ViewModelHelper.IsEverythingFilled(nwc_ViewModel))
            {
                return;
            }
            Logger logger = new(nwc_ViewModel.FolderPath);
            NWCHelper.BatchExportModels(uiApp, nwc_ViewModel, ref logger);

            string msg = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов.";
            logger.Dispose();
            ModelHelper.Finisher(nwc_ViewModel, "ExportNWCFinished", msg);
        }
    }

    public class EventHandlerIFCExportUiArg : RevitEventWrapper<IFC_ViewModel>
    {
        public override void Execute(UIApplication uiApp, IFC_ViewModel ifc_ViewModel)
        {
            if (!ViewModelHelper.IsEverythingFilled(ifc_ViewModel))
            {
                return;
            }
            Logger logger = new(ifc_ViewModel.FolderPath);
            IFCHelper.BatchExportModels(uiApp, ifc_ViewModel, ref logger);

            string msg = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов.";
            logger.Dispose();
            ModelHelper.Finisher(ifc_ViewModel, "ExportIFCFinished", msg);
        }
    }

    public class EventHandlerDetachModelsUiArg : RevitEventWrapper<DetachViewModel>
    {
        public override void Execute(UIApplication uiApp, DetachViewModel detachViewModel)
        {
            if (!ViewModelHelper.IsEverythingFilled(detachViewModel))
            {
                return;
            }

            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = [.. detachViewModel.ListBoxItems];

            uiApp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(ErrorSwallowersHelper.TaskDialogBoxShowingEvent);
            application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(ErrorSwallowersHelper.Application_FailuresProcessing);
            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();

                if (!File.Exists(filePath))
                {
                    string error = $"Файла {filePath} не существует. Ты совсем Туттуру?";
                    item.Background = Brushes.Red;
                    continue;
                }

                DetachModel(application, filePath, detachViewModel);
            }
            uiApp.DialogBoxShowing -= new EventHandler<DialogBoxShowingEventArgs>(ErrorSwallowersHelper.TaskDialogBoxShowingEvent);
            application.FailuresProcessing -= new EventHandler<FailuresProcessingEventArgs>(ErrorSwallowersHelper.Application_FailuresProcessing);

            ModelHelper.Finisher(detachViewModel, "DetachModelsFinished");
        }
        private static void DetachModel(Application application, string filePath, DetachViewModel detachViewModel)
        {
            Document document;
            BasicFileInfo fileInfo;
            bool isWorkshared;
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
            ModelHelper.DeleteAllLinks(document);
            string fileDetachedPath = "";
            switch (detachViewModel.RadionButtonMode)
            {
                case 1:
                    string folder = detachViewModel.FolderPath;
                    fileDetachedPath = folder + "\\" + document.Title.Replace("_detached", "").Replace("_отсоединено", "") + ".rvt";
                    break;
                case 2:
                    string maskIn = detachViewModel.MaskIn;
                    string maskOut = detachViewModel.MaskOut;
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

    public class EventHandlerTransmitModelsUiArg : RevitEventWrapper<TransmitViewModel>
    {
        public override void Execute(UIApplication uiApp, TransmitViewModel transmitViewModel)
        {
            if (!ViewModelHelper.IsEverythingFilled(transmitViewModel))
            {
                return;
            }

            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = [.. transmitViewModel.ListBoxItems];

            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();

                if (!File.Exists(filePath))
                {
                    string error = $"Файла {filePath} не существует. Ты совсем Туттуру?";
                    item.Background = Brushes.Red;
                    continue;
                }

                string folder = transmitViewModel.FolderPath;
                bool isSameFolder = transmitViewModel.IsSameFolder;

                string transmittedFilePath = folder + "\\" + filePath.Split('\\').Last();
                File.Copy(filePath, transmittedFilePath, true);
                ModelPath transmittedModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(transmittedFilePath);
                RevitLinksHelper.Unload(transmittedModelPath, isSameFolder, folder);
            }
            ModelHelper.Finisher(transmitViewModel, "TransmitModelsFinished");
        }
    }

    public class EventHandlerMigrateModelsUiArg : RevitEventWrapper<MigrateViewModel>
    {
        public override void Execute(UIApplication uiApp, MigrateViewModel migrateViewModel)
        {
            if (string.IsNullOrEmpty(migrateViewModel.ConfigPath)
                || !migrateViewModel.ConfigPath.EndsWith(".json", true, CultureInfo.InvariantCulture))
            {
                MessageBox.Show("Предоставьте ссылку на конфиг");
                return;
            }

            Dictionary<string, string> items;
            List<string> movedFiles = [];
            List<string> failedFiles = [];

            using (FileStream file = File.OpenRead(migrateViewModel.ConfigPath))
            {
                try
                {
                    items = JsonSerializer.Deserialize<Dictionary<string, string>>(file);
                }
                catch
                {
                    MessageBox.Show("Неверная схема файла");
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
                    MessageBox.Show(ex.Message);
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

            string msg = failedFiles.Count > 0
                ? $"Задание выполнено.\nСледующие файлы не были скопированы:\n{string.Join("\n", failedFiles)}"
                : "Задание выполнено.";

            ModelHelper.Finisher(migrateViewModel, "MigrateModelsFinished", msg);
        }
    }

    public class EventHandlerLinkModelsUiArg : RevitEventWrapper<LinkViewModel>
    {
        public override void Execute(UIApplication uiApp, LinkViewModel linkViewModel)
        {
            RevitLinksHelper.CreateLinks(uiApp, linkViewModel);
            ModelHelper.Finisher(linkViewModel, "LinkModelsFinished");
        }
    }
}