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

namespace VLS.BatchExportNet.Views.IFC
{
    static class IFCHelper
    {
        internal static void BatchExportModels(UIApplication uiApp, IFCExportUi ui, ref Logger logger)
        {
            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = [.. ui.listBoxItems];

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
                        WorksetConfiguration worksetConfiguration = ModelHelper.CloseWorksetsWithLinks(modelPath);
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
        private static void ExportModel(Document document, IFCExportUi ui, ref bool isFuckedUp, Logger logger)
        {
            Element view = default;
            using (FilteredElementCollector stuff = new(document))
            {
                view = stuff.OfClass(typeof(View3D)).FirstOrDefault(e => e.Name == ui.TextBoxExportScopeViewName.Text && !((View3D)e).IsTemplate);
            }

            if ((bool)ui.RadioButtonExportScopeView.IsChecked
                && ModelHelper.IsViewEmpty(document, view))
            {
                logger.Error("Нет геометрии на виде.");
                isFuckedUp = true;
            }
            else
            {
                IFCExportOptions iFCExportOptions = IFC_ExportOptions(document, ui);
                string folder = "";
                string prefix = "";
                string postfix = "";

                ui.Dispatcher.Invoke(() => folder = ui.TextBoxFolder.Text);
                ui.Dispatcher.Invoke(() => prefix = ui.TextBoxPrefix.Text);
                ui.Dispatcher.Invoke(() => postfix = ui.TextBoxPostfix.Text);

                string fileExportName = prefix + document.Title.Replace("_отсоединено", "") + postfix;
                string fileName = folder + "\\" + fileExportName + ".ifc";

                string oldHash = null;
                if (File.Exists(fileName))
                {
                    oldHash = ModelHelper.MD5Hash(fileName);
                    logger.Hash(oldHash);
                }

                using (Transaction transaction = new(document))
                {
                    transaction.Start("Экспорт IFCHelper");

                    try
                    {
                        document?.Export(folder, fileExportName, iFCExportOptions);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Смотри исключение.", ex);
                        isFuckedUp = true;
                    }
                    transaction.Commit();
                }

                iFCExportOptions.Dispose();

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
        private static IFCExportOptions IFC_ExportOptions(Document document, IFCExportUi batchExportIFC)
        {
            IFCExportOptions options = new()
            {
                ExportBaseQuantities = (bool)batchExportIFC.CheckBoxExportBaseQuantities.IsChecked,
                FamilyMappingFile = batchExportIFC.TextBoxMapping.Text,
                FileVersion = IFCExportUi
                    .indexToIFCVersion
                    .First(e => e.Key == batchExportIFC
                        .ComboBoxIFCVersion
                        .SelectedIndex)
                    .Value,
                FilterViewId = new FilteredElementCollector(document)
                .OfClass(typeof(View))
                .FirstOrDefault(e => e.Name == batchExportIFC
                    .TextBoxExportScopeViewName
                    .Text)
                .Id,
                SpaceBoundaryLevel = batchExportIFC.ComboBoxSpaceBoundaryLevel.SelectedIndex,
                WallAndColumnSplitting = (bool)batchExportIFC.CheckBoxWallAndColumnSplitting.IsChecked
            };

            return options;
        }
    }
}
