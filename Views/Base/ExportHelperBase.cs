using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.ApplicationServices;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.NWC;

namespace VLS.BatchExportNet.Views.Base
{
    public class ExportHelperBase
    {
        public void BatchExportModels
            (ViewModelBase_Extended viewModel, UIApplication uiApp, ref Logger logger)
        {
            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = [.. viewModel.ListBoxItems];

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
                        string[] prefixes = viewModel.WorksetPrefix
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
                    ExportModel(viewModel, document, ref isFuckedUp, ref logger);
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

        public virtual void ExportModel(ViewModelBase_Extended viewModel, Document document, ref bool isFuckedUp, ref Logger logger) { }
        public static void Export(ViewModelBase_Extended viewModel, Document document, object exportOptions, ref Logger logger, ref bool isFuckedUp)
        {
            string folderPath = viewModel.FolderPath;
            string fileExportName = viewModel.NamePrefix
                + document.Title.Replace("_отсоединено", "")
                + viewModel.NamePostfix;
            string fileFormat;

            if (exportOptions is NavisworksExportOptions)
                fileFormat = ".nwc";
            else
                fileFormat = ".ifc";

            string fileName = folderPath + "\\" + fileExportName + fileFormat;
            string oldHash = null;
            if (File.Exists(fileName))
            {
                oldHash = fileName.MD5Hash();
                logger.Hash(oldHash);
            }

            try
            {
                if (exportOptions is NavisworksExportOptions)
                {
                    NavisworksExportOptions navisworksExportOptions = exportOptions as NavisworksExportOptions;
                    document?.Export(folderPath, fileExportName, navisworksExportOptions);
                }
                else
                {
                    IFCExportOptions iFCExportOptions = exportOptions as IFCExportOptions;
                    document?.Export(folderPath, fileExportName, iFCExportOptions);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Смотри исключение.", ex);
                isFuckedUp = true;
            }

            if (!File.Exists(fileName))
            {
                logger.Error("Файл не был создан. Скорее всего нет геометрии на виде.");
                isFuckedUp = true;
                return;
            }
            string newHash = fileName.MD5Hash();
            logger.Hash(newHash);

            if (newHash == oldHash)
            {
                logger.Error("Файл не был обновлён. Хэш сумма не изменилась.");
                isFuckedUp = true;
            }
        }
        public static bool IsViewEmpty(ViewModelBase_Extended viewModel, Document document, ref Logger logger, ref bool isFuckedUp)
        {
            if (viewModel is NWC_ViewModel model && model.ExportLinks)
                return false;

            if (viewModel.ExportScopeView
                && document.IsViewEmpty(GetView(viewModel, document)))
            {
                logger.Error("Нет геометрии на виде.");
                isFuckedUp = true;
                return true;
            }

            return false;
        }
        private static Element GetView(ViewModelBase_Extended viewModel, Document document) =>
            new FilteredElementCollector(document)
                .OfClass(typeof(View3D))
                .FirstOrDefault(e => e.Name == viewModel.ViewName && !((View3D)e).IsTemplate);
    }
}