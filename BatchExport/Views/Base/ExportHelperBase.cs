﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.NWC;

namespace AlterTools.BatchExport.Views.Base
{
    public class ExportHelperBase
    {
        public void BatchExportModels
            (IConfigBase_Extended iConfig, UIApplication uiApp, ref Logger log)
        {
            using Application app = uiApp.Application;
            string[] models = iConfig.Files;
            ListBoxItem[] items = GetListBoxItems(iConfig);

            if (iConfig is ViewModelBase_Extended viewModel)
            {
                if (items is null) return;
                items = viewModel.ListBoxItems.ToArray();
                models = items.Select(e => e.Content.ToString()).ToArray();
            }

            foreach (string file in models)
            {
                log.LineBreak();
                DateTime startTime = DateTime.Now;
                log.Start(file);

                if (!File.Exists(file))
                {
                    HandleFileNotFound(file, items, log);
                    continue;
                }
                using ErrorSwallower errorSwallower = new(uiApp);
                Document doc = OpenDocument(file, app, iConfig, log, items);
                if (doc is null) continue;

                log.FileOpened();
                UpdateItemBackground(items, file, Brushes.Blue);

                bool isFuckedUp = false;

                try
                {
                    ExportModel(iConfig, doc, ref isFuckedUp, ref log);
                }
                catch (Exception ex)
                {
                    log.Error("Ля, я хз даже. Смотри, что в исключении написано: ", ex);
                    isFuckedUp = true;
                }
                finally
                {
                    CloseDocument(doc, ref isFuckedUp, items, file, log);
                    log.TimeForFile(startTime);
                    Thread.Sleep(500);
                }
            }

            log.LineBreak();
            log.ErrorTotal();
            log.TimeTotal();
        }
        private static ListBoxItem[] GetListBoxItems(IConfigBase_Extended iConfig)
            => iConfig is ViewModelBase_Extended viewModel
                ? viewModel.ListBoxItems.ToArray() : null;
        private static void HandleFileNotFound(string file, ListBoxItem[] items, Logger log)
        {
            log.Error($"Файла {file} не существует. Ты совсем Туттуру?");
            UpdateItemBackground(items, file, Brushes.Red);
        }
        private static void UpdateItemBackground(ListBoxItem[] items, string file, Brush color)
        {
            ListBoxItem item = items.FirstOrDefault(e => e.Content.ToString() == file);
            if (item is not null) item.Background = color;
        }
        private static Document OpenDocument(string file, Application app, IConfigBase_Extended iConfig, Logger log, ListBoxItem[] items)
        {
            try
            {
                BasicFileInfo fileInfo = BasicFileInfo.Extract(file);
                ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(file);

                TransmissionData trData = File.Exists(fileInfo.CentralPath) //ensures that central model exists and reachable
                    ? TransmissionData.ReadTransmissionData(modelPath)
                    : null;
                bool transmitted = trData is not null && trData.IsTransmitted;

                WorksetConfiguration worksetConfiguration = fileInfo.IsWorkshared
                    ? (file.Equals(fileInfo.CentralPath) && !transmitted && iConfig.WorksetPrefixes.Length != 0
                        ? modelPath.CloseWorksetsWithLinks(iConfig.WorksetPrefixes)
                        : new WorksetConfiguration())
                    : null;

                return worksetConfiguration is null
                    ? app.OpenDocumentFile(file)
                    : modelPath.OpenDetached(app, worksetConfiguration);
            }
            catch (Exception ex)
            {
                log.Error("Файл не открылся. ", ex);
                UpdateItemBackground(items, file, Brushes.Red);
                return null;
            }
        }
        private static void CloseDocument(Document doc, ref bool isFuckedUp, ListBoxItem[] items, string file, Logger log)
        {
            if (doc is null) return;

            try
            {
                doc.FreeTheModel();
                if (!isFuckedUp)
                    log.Success("Всё ок.");
            }
            catch (Exception ex)
            {
                log.Error("Не смог освободить рабочие наборы. ", ex);
                isFuckedUp = true;
            }
            finally
            {
                doc.Close(false);
                doc.Dispose();
                UpdateItemBackground(items, file, isFuckedUp ? Brushes.Red : Brushes.Green);
            }
        }

        public virtual void ExportModel(IConfigBase_Extended iConfig, Document doc, ref bool isFuckedUp, ref Logger log) { }
        public static void Export(IConfigBase_Extended iConfig,
            Document doc, object options,
            ref Logger log, ref bool isFuckedUp)
        {
            string folderPath = iConfig.FolderPath;
            string fileExportName = $"{iConfig.NamePrefix}" +
                $"{doc.Title.RemoveDetach()}" +
                $"{iConfig.NamePostfix}";
            string fileWithExtension = $"{fileExportName}" +
                $"{(options is NavisworksExportOptions ? ".nwc" : ".ifc")}";
            string fileName = Path.Combine(folderPath, fileWithExtension);

            string oldHash = File.Exists(fileName) ? fileName.MD5Hash() : null;
            if (oldHash is not null) log.Hash(oldHash);

            try
            {
                if (options is NavisworksExportOptions navisOptions)
                    doc?.Export(folderPath, fileExportName, navisOptions);
                else
                    doc?.Export(folderPath, fileExportName, options as IFCExportOptions);
            }
            catch (Exception ex)
            {
                log.Error("Смотри исключение.", ex);
                isFuckedUp = true;
                return;
            }

            if (!File.Exists(fileName))
            {
                log.Error("Файл не был создан. Скорее всего нет геометрии на виде.");
                isFuckedUp = true;
                return;
            }

            string newHash = fileName.MD5Hash();
            log.Hash(newHash);

            if (newHash == oldHash)
            {
                log.Error("Файл не был обновлён. Хэш сумма не изменилась.");
                isFuckedUp = true;
            }
        }
        public static bool IsViewEmpty(IConfigBase_Extended iConfig, Document doc, ref Logger log, ref bool isFuckedUp)
        {
            if (iConfig is NWC_ViewModel model && model.ExportLinks) return false;

            if (iConfig.ExportScopeView
                && doc.IsViewEmpty(GetView(iConfig, doc)))
            {
                log.Error("Нет геометрии на виде.");
                isFuckedUp = true;
                return true;
            }

            return false;
        }
        private static Element GetView(IConfigBase_Extended iConfig, Document doc)
            => new FilteredElementCollector(doc)
                .OfClass(typeof(View3D))
                .FirstOrDefault(e => e.Name == iConfig.ViewName && !((View3D)e).IsTemplate);
    }
}