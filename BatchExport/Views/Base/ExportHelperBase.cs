using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Utils.Logger;
using AlterTools.BatchExport.Views.NWC;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Views.Base;

public class ExportHelperBase
{
    public void BatchExportModels(IConfigBaseExtended iConfig, UIApplication uiApp, ref ILogger log)
    {
        using Application app = uiApp.Application;
        using ErrorSuppressor errorSuppressor = new(uiApp);

        string[] models = iConfig.Files;

        ListBoxItem[] items = GetListBoxItems(iConfig);
        if (items is null) return;

        if (iConfig is ViewModelBaseExtended)
        {
            models = items.Select(item => item.Content.ToString()).ToArray();
        }

        foreach (string file in models)
        {
            DateTime startTime = DateTime.Now;

            log.LineBreak();
            log.Start(file);

            if (!File.Exists(file))
            {
                HandleFileNotFound(file, items, log);
                continue;
            }

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

    private static ListBoxItem[] GetListBoxItems(IConfigBaseExtended iConfig)
    {
        return iConfig is ViewModelBaseExtended viewModel
            ? viewModel.ListBoxItems.ToArray()
            : null;
    }

    private static void HandleFileNotFound(string file, ListBoxItem[] items, ILogger log)
    {
        log.Error($"Файла {file} не существует. Ты совсем Туттуру?");
        UpdateItemBackground(items, file, Brushes.Red);
    }

    private static void UpdateItemBackground(ListBoxItem[] items, string file, Brush color)
    {
        ListBoxItem item = items.FirstOrDefault(i => i.Content.ToString() == file);
        if (item is not null)
        {
            item.Background = color;
        }
    }

    private static Document OpenDocument(string file, Application app, IConfigBaseExtended iConfig, ILogger log,
        ListBoxItem[] items)
    {
        try
        {
            BasicFileInfo fileInfo = BasicFileInfo.Extract(file);
            ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(file);

            TransmissionData trData =
                File.Exists(fileInfo.CentralPath) // ensure that central model exists and reachable
                    ? TransmissionData.ReadTransmissionData(modelPath)
                    : null;

            bool transmitted = trData is { IsTransmitted: true };

            WorksetConfiguration worksetConfiguration = fileInfo.IsWorkshared
                ? file.Equals(fileInfo.CentralPath)
                  && !transmitted
                  && iConfig.WorksetPrefixes.Length != 0
                    ? modelPath.CloseWorksets(iConfig.WorksetPrefixes)
                    : new WorksetConfiguration()
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

    private static void CloseDocument(Document doc, ref bool isFuckedUp, ListBoxItem[] items, string file, ILogger log)
    {
        if (doc is null) return;

        try
        {
            doc.FreeTheModel();
            if (!isFuckedUp)
            {
                log.Success("Всё ок.");
            }
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
            UpdateItemBackground(items,
                file,
                isFuckedUp ? Brushes.Red : Brushes.Green);
        }
    }

    protected virtual void ExportModel(IConfigBaseExtended iConfig,
        Document doc,
        ref bool isFuckedUp,
        ref ILogger log) { }

    protected static void Export(IConfigBaseExtended iConfig,
        Document doc,
        object options,
        ref ILogger log,
        ref bool isFuckedUp)
    {
        string folderPath = iConfig.FolderPath;

        string fileExportName = $"{iConfig.NamePrefix}" +
                                $"{doc.Title.RemoveDetach()}" +
                                $"{iConfig.NamePostfix}";

        string fileWithExtension = $"{fileExportName}" +
                                   $"{(options is NavisworksExportOptions ? ".nwc" : ".ifc")}";

        string fileName = Path.Combine(folderPath, fileWithExtension);
        string oldHash = File.Exists(fileName) ? fileName.GetMd5Hash() : null;

        if (oldHash is not null)
        {
            log.Hash(oldHash);
        }

        try
        {
            if (options is NavisworksExportOptions navisOptions)
            {
                doc.Export(folderPath, fileExportName, navisOptions);
            }
            else
            {
                doc.Export(folderPath, fileExportName, options as IFCExportOptions);
            }
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

        string newHash = fileName.GetMd5Hash();
        log.Hash(newHash);

        if (newHash != oldHash) return;

        log.Error("Файл не был обновлён. Хэш сумма не изменилась.");
        isFuckedUp = true;
    }

    protected static bool IsViewReadyForExport(IConfigBaseExtended iConfig, Document doc, ref ILogger log, ref bool isFuckedUp)
    {
        if (iConfig is NWCViewModel { ExportLinks: true }) return true;

        if (!iConfig.ExportScopeView) return true;
        
        if (!doc.IsViewEmpty(GetView(iConfig, doc))) return true;

        if (iConfig.IgnoreMissingView)
        {
            log.Info($"Вида {iConfig.ViewName} не существует. Экспорт будет выполнен по всей модели.");
            return true;
        }

        log.Error(doc.DoesViewExist(iConfig.ViewName)
            ? "Нет геометрии на виде."
            : $"Вида {iConfig.ViewName} не существует. Экспорт выполнен не будет.");

        isFuckedUp = true;
        return false;
    }

    private static Element GetView(IConfigBaseExtended iConfig, Document doc)
    {
        return new FilteredElementCollector(doc)
            .OfClass(typeof(View3D))
            .FirstOrDefault(el => el.Name == iConfig.ViewName && !((View3D)el).IsTemplate);
    }
}