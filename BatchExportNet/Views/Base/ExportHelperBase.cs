using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
            (IConfigBase_Extended iConfig, UIApplication uiApp, ref Logger logger)
        {
            using Application application = uiApp.Application;
            List<string> models = iConfig.Files;
            List<ListBoxItem> items = [];

            bool isVM = iConfig is ViewModelBase_Extended viewModel;
            if (isVM)
            {
                viewModel = iConfig as ViewModelBase_Extended;
                items = [.. viewModel.ListBoxItems];
                models = items.Select(e => e.Content.ToString()).ToList();
            }

            foreach (string file in models)
            {
                bool fileIsWorkshared = true;

                logger.LineBreak();
                DateTime startTime = DateTime.Now;
                logger.Start(file);

                if (!File.Exists(file))
                {
                    logger.Error($"Файла {file} не существует. Ты совсем Туттуру?");
                    if (isVM)
                        items.FirstOrDefault(e => e.Content.ToString() == file).Background = Brushes.Red;
                    continue;
                }
                using ErrorSwallower errorSwallower = new(uiApp, application);

                Document document;
                BasicFileInfo fileInfo;
                try
                {
                    fileInfo = BasicFileInfo.Extract(file);
                    if (!fileInfo.IsWorkshared)
                    {
                        document = application.OpenDocumentFile(file);
                        fileIsWorkshared = false;
                    }
                    else if (file.Equals(fileInfo.CentralPath))
                    {
                        ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(file);
                        string[] prefixes = iConfig.WorksetPrefixes;
                        WorksetConfiguration worksetConfiguration = modelPath.CloseWorksetsWithLinks(prefixes);
                        document = modelPath.OpenAsIs(application, worksetConfiguration);
                    }
                    else
                    {
                        ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(file);
                        WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.OpenAllWorksets);
                        document = modelPath.OpenAsIs(application, worksetConfiguration);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Файл не открылся. ", ex);
                    if (isVM)
                        items.FirstOrDefault(e => e.Content.ToString() == file).Background = Brushes.Red;
                    continue;
                }

                fileInfo.Dispose();
                logger.FileOpened();

                if (isVM)
                    items.FirstOrDefault(e => e.Content.ToString() == file).Background = Brushes.Blue;
                bool isFuckedUp = false;

                try
                {
                    ExportModel(iConfig, document, ref isFuckedUp, ref logger);
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
                        if (isVM)
                            items.FirstOrDefault(e => e.Content.ToString() == file).Background = Brushes.Red;
                    }
                    else
                    {
                        if (isVM)
                            items.FirstOrDefault(e => e.Content.ToString() == file).Background = Brushes.Green;
                        logger.Success("Всё ок.");
                    }

                    logger.TimeForFile(startTime);
                    Thread.Sleep(500);
                }
            }

            logger.LineBreak();
            logger.ErrorTotal();
            logger.TimeTotal();
        }

        public virtual void ExportModel(IConfigBase_Extended iConfig,
            Document document, ref bool isFuckedUp, ref Logger logger)
        { }
        public static void Export(IConfigBase_Extended iConfig,
            Document document, object exportOptions,
            ref Logger logger, ref bool isFuckedUp)
        {
            string folderPath = iConfig.FolderPath;
            string fileExportName = iConfig.NamePrefix
                + document.Title.Replace("_отсоединено", "")
                + iConfig.NamePostfix;
            string fileWithExtension = fileExportName;

            if (exportOptions is NavisworksExportOptions)
                fileWithExtension += ".nwc";
            else
                fileWithExtension += ".ifc";

            string fileName = Path.Combine(folderPath, fileWithExtension);
            string oldHash = null;
            if (File.Exists(fileName))
            {
                oldHash = fileName.MD5Hash();
                logger.Hash(oldHash);
            }

            try
            {
                if (exportOptions is NavisworksExportOptions)
                    document?.Export(folderPath, fileExportName, exportOptions as NavisworksExportOptions);
                else
                    document?.Export(folderPath, fileExportName, exportOptions as IFCExportOptions);
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
        public static bool IsViewEmpty(IConfigBase_Extended iConfig, Document document, ref Logger logger, ref bool isFuckedUp)
        {
            if (iConfig is NWC_ViewModel model && model.ExportLinks)
                return false;

            if (iConfig.ExportScopeView
                && document.IsViewEmpty(GetView(iConfig, document)))
            {
                logger.Error("Нет геометрии на виде.");
                isFuckedUp = true;
                return true;
            }

            return false;
        }
        private static Element GetView(IConfigBase_Extended iConfig, Document document) =>
            new FilteredElementCollector(document)
                .OfClass(typeof(View3D))
                .FirstOrDefault(e => e.Name == iConfig.ViewName && !((View3D)e).IsTemplate);
    }
}