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
            List<ListBoxItem> items = GetListBoxItems(iConfig);

            if (iConfig is ViewModelBase_Extended viewModel)
            {
                items = [.. viewModel.ListBoxItems];
                models = items.Select(e => e.Content.ToString()).ToList();
            }

            foreach (string file in models)
            {
                logger.LineBreak();
                DateTime startTime = DateTime.Now;
                logger.Start(file);

                if (!File.Exists(file))
                {
                    HandleFileNotFound(file, items, logger);
                    continue;
                }
                using ErrorSwallower errorSwallower = new(uiApp, application);

                Document document = OpenDocument(file, application, iConfig, logger, items);
                if (document is null) continue;

                logger.FileOpened();
                UpdateItemBackground(items, file, Brushes.Blue);

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
                    CloseDocument(document, ref isFuckedUp, items, file, logger);
                    logger.TimeForFile(startTime);
                    Thread.Sleep(500);
                }
            }

            logger.LineBreak();
            logger.ErrorTotal();
            logger.TimeTotal();
        }
        private static List<ListBoxItem> GetListBoxItems(IConfigBase_Extended iConfig) =>
            iConfig is ViewModelBase_Extended viewModel
                ? new List<ListBoxItem>(viewModel.ListBoxItems)
                : [];
        private static void HandleFileNotFound(string file, List<ListBoxItem> items, Logger logger)
        {
            logger.Error($"Файла {file} не существует. Ты совсем Туттуру?");
            UpdateItemBackground(items, file, Brushes.Red);
        }
        private static void UpdateItemBackground(List<ListBoxItem> items, string file, Brush color)
        {
            ListBoxItem item = items.FirstOrDefault(e => e.Content.ToString() == file);
            if (item is not null) item.Background = color;
        }
        private static Document OpenDocument(string file, Application application, IConfigBase_Extended iConfig, Logger logger, List<ListBoxItem> items)
        {
            try
            {
                BasicFileInfo fileInfo = BasicFileInfo.Extract(file);
                ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(file);

                using TransmissionData trData = TransmissionData.ReadTransmissionData(modelPath);
                bool transmitted = trData is not null && trData.IsTransmitted;

                WorksetConfiguration worksetConfiguration = fileInfo.IsWorkshared
                    ? (file.Equals(fileInfo.CentralPath) && !transmitted
                        ? modelPath.CloseWorksetsWithLinks(iConfig.WorksetPrefixes)
                        : new WorksetConfiguration(WorksetConfigurationOption.OpenAllWorksets))
                    : null;

                return worksetConfiguration is null
                    ? application.OpenDocumentFile(file)
                    : modelPath.OpenAsIs(application, worksetConfiguration);
            }
            catch (Exception ex)
            {
                logger.Error("Файл не открылся. ", ex);
                UpdateItemBackground(items, file, Brushes.Red);
                return null;
            }
        }
        private static void CloseDocument(Document document, ref bool isFuckedUp, List<ListBoxItem> items, string file, Logger logger)
        {
            if (document is null) return;

            try
            {
                document.FreeTheModel();
                logger.Success("Всё ок.");
            }
            catch (Exception ex)
            {
                logger.Error("Не смог освободить рабочие наборы. ", ex);
                isFuckedUp = true;
            }
            finally
            {
                document.Close(false);
                document.Dispose();
                UpdateItemBackground(items, file, isFuckedUp ? Brushes.Red : Brushes.Green);
            }
        }

        public virtual void ExportModel(IConfigBase_Extended iConfig,
            Document document, ref bool isFuckedUp, ref Logger logger)
        { }
        public static void Export(IConfigBase_Extended iConfig,
            Document document, object exportOptions,
            ref Logger logger, ref bool isFuckedUp)
        {
            string folderPath = iConfig.FolderPath;
            string fileExportName = $"{iConfig.NamePrefix}" +
                $"{document.Title.Replace("_отсоединено", "").Replace("_detached", "")}" +
                $"{iConfig.NamePostfix}";
            string fileWithExtension = $"{fileExportName}" +
                $"{(exportOptions is NavisworksExportOptions ? ".nwc" : ".ifc")}";
            string fileName = Path.Combine(folderPath, fileWithExtension);

            string oldHash = File.Exists(fileName) ? fileName.MD5Hash() : null;
            if (oldHash is not null) logger.Hash(oldHash);

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
                return;
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
            if (iConfig is NWC_ViewModel model && model.ExportLinks) return false;

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