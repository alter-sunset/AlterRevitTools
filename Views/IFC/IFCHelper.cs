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
        internal static void BatchExportModels(this IFC_ViewModel ifc_ViewModel, UIApplication uiApp, ref Logger logger)
        {
            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = [.. ifc_ViewModel.ListBoxItems];

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
                        string[] prefixes = ifc_ViewModel.WorksetPrefix
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
                    ifc_ViewModel.ExportModel(document, ref isFuckedUp, logger);
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
        private static void ExportModel(this IFC_ViewModel ifc_ViewModel, Document document, ref bool isFuckedUp, Logger logger)
        {
            Element view = default;
            using (FilteredElementCollector stuff = new(document))
            {
                view = stuff.OfClass(typeof(View3D)).FirstOrDefault(e => e.Name == ifc_ViewModel.ViewName && !((View3D)e).IsTemplate);
            }

            if (ifc_ViewModel.ExportScopeView
                && document.IsViewEmpty(view))
            {
                logger.Error("Нет геометрии на виде.");
                isFuckedUp = true;
            }
            else
            {
                IFCExportOptions iFCExportOptions = ifc_ViewModel.IFC_ExportOptions(document);
                string folder = ifc_ViewModel.FolderPath;
                string prefix = ifc_ViewModel.NamePrefix;
                string postfix = ifc_ViewModel.NamePostfix;

                string fileExportName = prefix + document.Title.Replace("_отсоединено", "") + postfix;
                string fileName = folder + "\\" + fileExportName + ".ifc";

                string oldHash = null;
                if (File.Exists(fileName))
                {
                    oldHash = fileName.MD5Hash();
                    logger.Hash(oldHash);
                }

                using (Transaction transaction = new(document))
                {
                    transaction.Start("Экспорт IFC");

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
        private static IFCExportOptions IFC_ExportOptions(this IFC_ViewModel ifc_ViewModel, Document document)
        {
            IFCExportOptions options = new()
            {
                ExportBaseQuantities = ifc_ViewModel.ExportBaseQuantities,
                FamilyMappingFile = ifc_ViewModel.Mapping,
                FileVersion = ifc_ViewModel.SelectedVersion.Key,
                FilterViewId = new FilteredElementCollector(document)
                .OfClass(typeof(View))
                .FirstOrDefault(e => e.Name == ifc_ViewModel.ViewName)
                .Id,
                SpaceBoundaryLevel = ifc_ViewModel.SelectedLevel.Key,
                WallAndColumnSplitting = ifc_ViewModel.WallAndColumnSplitting
            };

            return options;
        }
    }
}