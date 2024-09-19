using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.ApplicationServices;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Views.NWC
{
    static class NWCHelper
    {
        internal static void BatchExportModels(this NWC_ViewModel nwc_ViewModel, UIApplication uiApp, ref Logger logger)
        {
            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = [.. nwc_ViewModel.ListBoxItems];

            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();
                bool fileIsWorkshared = true;

                logger.LineBreak();
                DateTime startTime = DateTime.Now;
                logger.Start(filePath);

                if (!File.Exists(filePath))
                {
                    logger.Error($"Файла {filePath} не существует. Ты совсем Туттуру?");
                    item.Background = Brushes.Red;
                    continue;
                }
                uiApp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(ErrorSwallower.TaskDialogBoxShowingEvent);
                application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(ErrorSwallower.Application_FailuresProcessing);

                Document document;
                BasicFileInfo fileInfo;
                try
                {
                    fileInfo = BasicFileInfo.Extract(filePath);
                    if (!fileInfo.IsWorkshared)
                    {
                        document = application.OpenDocumentFile(filePath);
                        fileIsWorkshared = false;
                    }
                    else if (filePath.Equals(fileInfo.CentralPath))
                    {
                        ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                        string[] prefixes = nwc_ViewModel.WorksetPrefix
                            .Split(';')
                            .Select(s => s.Trim())
                            .Where(e => !string.IsNullOrEmpty(e))
                            .ToArray();
                        WorksetConfiguration worksetConfiguration = modelPath.CloseWorksetsWithLinks(prefixes);
                        document = modelPath.OpenAsIs(application, worksetConfiguration);
                    }
                    else
                    {
                        ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                        WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.OpenAllWorksets);
                        document = modelPath.OpenAsIs(application, worksetConfiguration);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Файл не открылся. ", ex);
                    item.Background = Brushes.Red;
                    continue;
                }

                fileInfo.Dispose();
                logger.FileOpened();

                item.Background = Brushes.Blue;
                bool isFuckedUp = false;

                try
                {
                    nwc_ViewModel.ExportModel(document, ref isFuckedUp, logger);
                }
                catch (Exception ex)
                {
                    logger.Error("Ля, я хз даже. Смотри, что в исключении написано: ", ex);
                    isFuckedUp = true;
                }
                finally
                {
                    if (fileIsWorkshared)
                    {
                        try
                        {
                            document.FreeTheModel();
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Не смог освободить рабочие наборы. ", ex);
                            isFuckedUp = true;
                        }
                    }

                    document?.Close(false);
                    document?.Dispose();

                    if (isFuckedUp)
                    {
                        item.Background = Brushes.Red;
                    }
                    else
                    {
                        item.Background = Brushes.Green;
                        logger.Success("Всё ок.");
                    }

                    uiApp.DialogBoxShowing -= new EventHandler<DialogBoxShowingEventArgs>(ErrorSwallower.TaskDialogBoxShowingEvent);
                    application.FailuresProcessing -= new EventHandler<FailuresProcessingEventArgs>(ErrorSwallower.Application_FailuresProcessing);
                    logger.TimeForFile(startTime);
                    Thread.Sleep(500);
                }
            }

            logger.LineBreak();
            logger.ErrorTotal();
            logger.TimeTotal();
        }
        private static void ExportModel(this NWC_ViewModel nwc_ViewModel, Document document, ref bool isFuckedUp, Logger logger)
        {
            Element view = default;

            using (FilteredElementCollector stuff = new(document))
            {
                view = stuff.OfClass(typeof(View3D)).FirstOrDefault(e => e.Name == nwc_ViewModel.ViewName && !((View3D)e).IsTemplate);
            }

            if (nwc_ViewModel.ExportScopeView
                && !nwc_ViewModel.ExportLinks
                && document.IsViewEmpty(view))
            {
                logger.Error("Нет геометрии на виде.");
                isFuckedUp = true;
            }
            else
            {
                NavisworksExportOptions navisworksExportOptions = nwc_ViewModel.NWC_ExportOptions(document);
                string folderPath = nwc_ViewModel.FolderPath;
                string namePrefix = nwc_ViewModel.NamePrefix;
                string namePostfix = nwc_ViewModel.NamePostfix;
                string fileExportName = namePrefix + document.Title.Replace("_отсоединено", "") + namePostfix;
                string fileName = folderPath + "\\" + fileExportName + ".nwc";

                string oldHash = null;

                if (File.Exists(fileName))
                {
                    oldHash = fileName.MD5Hash();
                    logger.Hash(oldHash);
                }

                try
                {
                    document?.Export(folderPath, fileExportName, navisworksExportOptions);
                }
                catch (Exception ex)
                {
                    logger.Error("Смотри исключение.", ex);
                    isFuckedUp = true;
                }

                navisworksExportOptions.Dispose();

                if (!File.Exists(fileName))
                {
                    logger.Error("Файл не был создан. Скорее всего нет геометрии на виде.");
                    isFuckedUp = true;
                }
                else
                {
                    string newHash = fileName.MD5Hash();
                    logger.Hash(newHash);

                    if (newHash == oldHash)
                    {
                        logger.Error("Файл не был обновлён. Хэш сумма не изменилась.");
                        isFuckedUp = true;
                    }
                }

                view?.Dispose();
            }
        }
        private static NavisworksExportOptions NWC_ExportOptions(this NWC_ViewModel nwc_ViewModel, Document document)
        {
            NavisworksExportOptions options = new()
            {
                ConvertElementProperties = nwc_ViewModel.ConvertElementProperties,
                DivideFileIntoLevels = nwc_ViewModel.DivideFileIntoLevels,
                ExportElementIds = nwc_ViewModel.ExportElementIds,
                ExportLinks = nwc_ViewModel.ExportLinks,
                ExportParts = nwc_ViewModel.ExportParts,
                ExportRoomAsAttribute = nwc_ViewModel.ExportRoomAsAttribute,
                ExportRoomGeometry = nwc_ViewModel.ExportRoomGeometry,
                ExportUrls = nwc_ViewModel.ExportUrls,
                FindMissingMaterials = nwc_ViewModel.FindMissingMaterials,
                ConvertLights = nwc_ViewModel.ConvertLights,
                ConvertLinkedCADFormats = nwc_ViewModel.ConvertLinkedCADFormats,
                Coordinates = nwc_ViewModel.SelectedCoordinates.Key,
                Parameters = nwc_ViewModel.SelectedParameters.Key,
                FacetingFactor = double
                    .TryParse(nwc_ViewModel.FacetingFactor, out double facetingFactor)
                    ? facetingFactor
                    : 1.0,
                ExportScope = nwc_ViewModel.ExportScopeView
                    ? NavisworksExportScope.View
                    : NavisworksExportScope.Model

            };
            if (nwc_ViewModel.ExportScopeView)
                options.ViewId = new FilteredElementCollector(document)
                        .OfClass(typeof(View3D))
                        .FirstOrDefault(e => e.Name == nwc_ViewModel.ViewName
                            && !((View3D)e).IsTemplate)
                        .Id;
            return options;
        }
    }
}