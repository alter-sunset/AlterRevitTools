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
        internal static void BatchExportModels(UIApplication uiApp, NWCExportUi ui, ref Logger logger)
        {
            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = ui.listBoxItems.ToList();

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
                uiApp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(ErrorSwallowersHelper.TaskDialogBoxShowingEvent);
                application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(ErrorSwallowersHelper.Application_FailuresProcessing);

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
                        string[] prefixes = ui.TextBoxWorksetPrefix
                            .Text.Split(';')
                            .Select(s => s.Trim())
                            .Where(e => !string.IsNullOrEmpty(e))
                            .ToArray();
                        WorksetConfiguration worksetConfiguration = ModelHelper.CloseWorksetsWithLinks(modelPath, prefixes);
                        document = OpenDocumentHelper.OpenAsIs(application, modelPath, worksetConfiguration);
                    }
                    else
                    {
                        ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                        WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.OpenAllWorksets);
                        document = OpenDocumentHelper.OpenAsIs(application, modelPath, worksetConfiguration);
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
                    ExportModel(document, ui, ref isFuckedUp, logger);
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
                            ModelHelper.FreeTheModel(document);
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

                    uiApp.DialogBoxShowing -= new EventHandler<DialogBoxShowingEventArgs>(ErrorSwallowersHelper.TaskDialogBoxShowingEvent);
                    application.FailuresProcessing -= new EventHandler<FailuresProcessingEventArgs>(ErrorSwallowersHelper.Application_FailuresProcessing);
                    logger.TimeForFile(startTime);
                    Thread.Sleep(500);
                }
            }

            logger.LineBreak();
            logger.ErrorTotal();
            logger.TimeTotal();
        }
        private static void ExportModel(Document document, NWCExportUi ui, ref bool isFuckedUp, Logger logger)
        {
            Element view = default;

            using (FilteredElementCollector stuff = new(document))
            {
                view = stuff.OfClass(typeof(View3D)).FirstOrDefault(e => e.Name == ui.TextBoxExportScopeViewName.Text && !((View3D)e).IsTemplate);
            }

            if ((bool)ui.RadioButtonExportScopeView.IsChecked
                && !(bool)ui.CheckBoxExportLinks.IsChecked
                && ModelHelper.IsViewEmpty(document, view))
            {
                logger.Error("Нет геометрии на виде.");
                isFuckedUp = true;
            }
            else
            {
                NavisworksExportOptions navisworksExportOptions = NWC_ExportOptions(document, ui);
                string folder = "";
                string prefix = "";
                string postfix = "";

                ui.Dispatcher.Invoke(() => folder = ui.TextBoxFolder.Text);
                ui.Dispatcher.Invoke(() => prefix = ui.TextBoxPrefix.Text);
                ui.Dispatcher.Invoke(() => postfix = ui.TextBoxPostfix.Text);

                string fileExportName = prefix + document.Title.Replace("_отсоединено", "") + postfix;
                string fileName = folder + "\\" + fileExportName + ".nwc";

                string oldHash = null;

                if (File.Exists(fileName))
                {
                    oldHash = ModelHelper.MD5Hash(fileName);
                    logger.Hash(oldHash);
                }

                try
                {
                    document?.Export(folder, fileExportName, navisworksExportOptions);
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
                    string newHash = ModelHelper.MD5Hash(fileName);
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
        private static NavisworksExportOptions NWC_ExportOptions(Document document, NWCExportUi batchExportNWC)
        {
            string coordinates = ((ComboBoxItem)batchExportNWC.ComboBoxCoordinates.SelectedItem).Content.ToString();
            bool exportScope = (bool)batchExportNWC.RadioBattonExportScopeModel.IsChecked;
            string parameters = ((ComboBoxItem)batchExportNWC.ComboBoxParameters.SelectedItem).Content.ToString();

            if (double.TryParse(batchExportNWC.TextBoxFacetingFactor.Text, out double facetingFactor))
            {
                facetingFactor = double.Parse(batchExportNWC.TextBoxFacetingFactor.Text);
            }
            else
            {
                facetingFactor = 1.0;
            }

            NavisworksExportOptions options = new()
            {
                ConvertElementProperties = (bool)batchExportNWC.CheckBoxConvertElementProperties.IsChecked,
                DivideFileIntoLevels = (bool)batchExportNWC.CheckBoxDivideFileIntoLevels.IsChecked,
                ExportElementIds = (bool)batchExportNWC.CheckBoxExportElementIds.IsChecked,
                ExportLinks = (bool)batchExportNWC.CheckBoxExportLinks.IsChecked,
                ExportParts = (bool)batchExportNWC.CheckBoxExportParts.IsChecked,
                ExportRoomAsAttribute = (bool)batchExportNWC.CheckBoxExportRoomAsAttribute.IsChecked,
                ExportRoomGeometry = (bool)batchExportNWC.CheckBoxExportRoomGeometry.IsChecked,
                ExportUrls = (bool)batchExportNWC.CheckBoxExportUrls.IsChecked,
                FindMissingMaterials = (bool)batchExportNWC.CheckBoxFindMissingMaterials.IsChecked,
                ConvertLights = (bool)batchExportNWC.CheckBoxConvertLights.IsChecked,
                ConvertLinkedCADFormats = (bool)batchExportNWC.CheckBoxConvertLinkedCADFormats.IsChecked,
                FacetingFactor = facetingFactor
            };

            switch (coordinates)
            {
                case "Общие":
                    options.Coordinates = NavisworksCoordinates.Shared;
                    break;
                case "Внутренние для проекта":
                    options.Coordinates = NavisworksCoordinates.Internal;
                    break;
            }

            switch (exportScope)
            {
                case true:
                    options.ExportScope = NavisworksExportScope.Model;
                    break;
                case false:
                    options.ExportScope = NavisworksExportScope.View;
                    options.ViewId = new FilteredElementCollector(document)
                        .OfClass(typeof(View3D))
                        .FirstOrDefault(e => e.Name == batchExportNWC.TextBoxExportScopeViewName.Text && !((View3D)e).IsTemplate)
                        .Id;
                    break;
            }

            switch (parameters)
            {
                case "Все":
                    options.Parameters = NavisworksParameters.All;
                    break;
                case "Объекты":
                    options.Parameters = NavisworksParameters.Elements;
                    break;
                case "Нет":
                    options.Parameters = NavisworksParameters.None;
                    break;
            }

            return options;
        }
    }
}
