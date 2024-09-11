using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading;
using VLS.BatchExportNet.IFC;
using VLS.BatchExportNet.NWC;
using VLS.BatchExportNet.Link;
using VLS.BatchExportNet.Detach;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace VLS.BatchExportNet.Utils
{
    public static class Methods
    {
        public static void BatchExportNWC(UIApplication uiApp, NWCExportUi ui, ref Logger logger)
        {
            Autodesk.Revit.ApplicationServices.Application application = uiApp.Application;
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
                uiApp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(TaskDialogBoxShowingEvent);
                application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(Application_FailuresProcessing);
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
                        ModelPath modelPath = new FilePath(filePath);
                        WorksetConfiguration worksetConfiguration = CloseWorksetsWithLinks(modelPath);
                        document = OpenDocument.OpenAsIs(application, modelPath, worksetConfiguration);
                    }
                    else
                    {
                        ModelPath modelPath = new FilePath(filePath);
                        WorksetConfiguration worksetConfiguration = new WorksetConfiguration(WorksetConfigurationOption.OpenAllWorksets);
                        document = OpenDocument.OpenAsIs(application, modelPath, worksetConfiguration);
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
                    ExportModelToNWC(document, ui, ref isFuckedUp, logger);
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
                            FreeTheModel(document);
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

                    uiApp.DialogBoxShowing -= new EventHandler<DialogBoxShowingEventArgs>(TaskDialogBoxShowingEvent);
                    application.FailuresProcessing -= new EventHandler<FailuresProcessingEventArgs>(Application_FailuresProcessing);
                    logger.TimeForFile(startTime);
                    Thread.Sleep(500);
                }
            }

            application.Dispose();

            logger.LineBreak();
            logger.ErrorTotal();
            logger.TimeTotal();
            logger.Dispose();
        }
        public static void ExportModelToNWC(Document document, NWCExportUi ui, ref bool isFuckedUp, Logger logger)
        {
            Element view = default;

            using (FilteredElementCollector stuff = new(document))
            {
                view = stuff.OfClass(typeof(View3D)).FirstOrDefault(e => e.Name == ui.TextBoxExportScopeViewName.Text && !((View3D)e).IsTemplate);
            }

            if ((bool)ui.RadioButtonExportScopeView.IsChecked
                && !(bool)ui.CheckBoxExportLinks.IsChecked
                && IsViewEmpty(document, view))
            {
                logger.Error("Нет геометрии на виде.");
                isFuckedUp = true;
            }
            else
            {
                NavisworksExportOptions navisworksExportOptions = ExportModelToNWCOptions(document, ui);
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
                    oldHash = UiExtMethods.MD5Hash(fileName);
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
                    string newHash = UiExtMethods.MD5Hash(fileName);
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
        public static NavisworksExportOptions ExportModelToNWCOptions(Document document, NWCExportUi batchExportNWC)
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

            NavisworksExportOptions options = new NavisworksExportOptions()
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
        public static void BatchExportIFC(UIApplication uiApp, IFCExportUi ui, ref Logger logger)
        {
            Application application = uiApp.Application;

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
                uiApp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(TaskDialogBoxShowingEvent);
                application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(Application_FailuresProcessing);
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
                        ModelPath modelPath = new FilePath(filePath);
                        WorksetConfiguration worksetConfiguration = CloseWorksetsWithLinks(modelPath);
                        document = OpenDocument.OpenAsIs(application, modelPath, worksetConfiguration);
                    }
                    else
                    {
                        ModelPath modelPath = new FilePath(filePath);
                        WorksetConfiguration worksetConfiguration = new WorksetConfiguration(WorksetConfigurationOption.OpenAllWorksets);
                        document = OpenDocument.OpenAsIs(application, modelPath, worksetConfiguration);
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
                    ExportModelToIFC(document, ui, ref isFuckedUp, logger);
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
                            FreeTheModel(document);
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

                    uiApp.DialogBoxShowing -= new EventHandler<DialogBoxShowingEventArgs>(TaskDialogBoxShowingEvent);
                    application.FailuresProcessing -= new EventHandler<FailuresProcessingEventArgs>(Application_FailuresProcessing);
                    logger.TimeForFile(startTime);
                    Thread.Sleep(500);
                }
            }

            application.Dispose();

            logger.LineBreak();
            logger.ErrorTotal();
            logger.TimeTotal();
            logger.Dispose();
        }
        public static void ExportModelToIFC(Document document, IFCExportUi ui, ref bool isFuckedUp, Logger logger)
        {
            Element view = default;
            using (FilteredElementCollector stuff = new(document))
            {
                view = stuff.OfClass(typeof(View3D)).FirstOrDefault(e => e.Name == ui.TextBoxExportScopeViewName.Text && !((View3D)e).IsTemplate);
            }

            if ((bool)ui.RadioButtonExportScopeView.IsChecked
                && IsViewEmpty(document, view))
            {
                logger.Error("Нет геометрии на виде.");
                isFuckedUp = true;
            }
            else
            {
                IFCExportOptions iFCExportOptions = new();
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
                    oldHash = UiExtMethods.MD5Hash(fileName);
                    logger.Hash(oldHash);
                }

                using (Transaction transaction = new Transaction(document))
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
                    string newHash = UiExtMethods.MD5Hash(fileName);
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
        public static IFCExportOptions IFCExportOptions(Document document, IFCExportUi batchExportIFC)
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
        public static void LinkRevitModel(UIApplication uiApp, LinkModelsUi ui)
        {
            //forgot to check for shared coord site
            Application application = uiApp.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            bool isCurrentWorkset = (bool)ui.CheckBoxCurrentWorkset.IsChecked;
            List<ListBoxItem> listItems = @ui.listBoxItems.ToList();

            ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(doc.PathName);
            IList<WorksetPreview> worksets = null;
            WorksetTable worksetTable = null;
            if (!isCurrentWorkset)
            {
                try
                {
                    worksets = WorksharingUtils.GetUserWorksetInfo(modelPath);
                    worksetTable = doc.GetWorksetTable();
                }
                catch
                {
                    isCurrentWorkset = false;
                }
            }
            RevitLinkOptions options = new RevitLinkOptions(false);

            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();

                if (!File.Exists(filePath))
                {
                    continue;
                }

                using Transaction t = new(doc);
                t.Start($"Link {filePath}");

                if (!isCurrentWorkset && worksets is not null && worksetTable is not null)
                {
                    WorksetId worksetId = worksets.FirstOrDefault(e => filePath.Contains(e.Name.Split('_')[0])).Id;
                    worksetTable.SetActiveWorksetId(worksetId);
                }

                ModelPath linkPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                try
                {
                    LinkLoadResult linkLoadResult = RevitLinkType.Create(doc, linkPath, options);
                    RevitLinkInstance revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, ImportPlacement.Shared);
                }
                catch
                {
                    t.RollBack();
                    continue;
                }

                t.Commit();
            }
        }
        public static WorksetConfiguration CloseWorksetsWithLinks(ModelPath modelPath)
        {
            WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.OpenAllWorksets);

            IList<WorksetPreview> worksets = WorksharingUtils.GetUserWorksetInfo(modelPath);
            IList<WorksetId> worksetIds = new List<WorksetId>();

            foreach (WorksetPreview worksetPreview in worksets)
            {
                if (worksetPreview.Name.StartsWith("99") || worksetPreview.Name.StartsWith("00"))
                {
                    worksetIds.Add(worksetPreview.Id);
                }
            }

            worksetConfiguration.Close(worksetIds);
            return worksetConfiguration;
        }
        public static void TaskDialogBoxShowingEvent(object sender, DialogBoxShowingEventArgs e)
        {
            TaskDialogShowingEventArgs e2 = e as TaskDialogShowingEventArgs;

            string dialogId = e2.DialogId;
            bool isConfirm = false;
            int dialogResult = 0;

            switch (dialogId)
            {
                case "TaskDialog_Missing_Third_Party_Updaters":
                case "TaskDialog_Missing_Third_Party_Updater":
                    isConfirm = true;
                    dialogResult = (int)TaskDialogResult.CommandLink1;
                    break;
                //case "TaskDialog_Cannot_Find_Central_Model":
                default:
                    isConfirm = true;
                    dialogResult = (int)TaskDialogResult.Close;
                    break;
            }

            if (isConfirm)
                e2.OverrideResult(dialogResult);
        }
        internal static bool IsViewEmpty(Document document, Element element)
        {
            View3D view = element as View3D;
            try
            {
                using FilteredElementCollector collector = new FilteredElementCollector(document, view.Id);
                return !collector.Where(e => e.Category != null && e.GetType() != typeof(RevitLinkInstance)).Any(e => e.CanBeHidden(view));
            }
            catch
            {
                return true;
            }
        }
        public static void Application_FailuresProcessing(object sender, FailuresProcessingEventArgs e)
        {
            FailuresAccessor failuresAccessor = e.GetFailuresAccessor();
            FailureProcessingResult response = PreprocessFailures(failuresAccessor);
            e.SetProcessingResult(response);
        }
        private static FailureProcessingResult PreprocessFailures(FailuresAccessor a)
        {
            IList<FailureMessageAccessor> failures
              = a.GetFailureMessages();

            foreach (FailureMessageAccessor f in failures)
            {
                FailureSeverity fseverity = a.GetSeverity();

                if (fseverity == FailureSeverity.Warning)
                {
                    a.DeleteWarning(f);
                }
                else
                {
                    a.ResolveFailure(f);
                    return FailureProcessingResult.ProceedWithCommit;
                }
            }
            return FailureProcessingResult.Continue;
        }
        public static void DeleteLinks(Document document)
        {
            using Transaction transaction = new(document);
            transaction.Start("Delete links from model");

            FailureHandlingOptions failOpt = transaction.GetFailureHandlingOptions();
            failOpt.SetFailuresPreprocessor(new CopyWatchAlertSwallower());
            transaction.SetFailureHandlingOptions(failOpt);

            List<Element> links = new FilteredElementCollector(document).OfClass(typeof(RevitLinkType)).ToList();

            foreach (Element link in links)
            {
                document.Delete(link.Id);
            }
            transaction.Commit();
        }
        public static void DetachModel(Application application, string filePath, DetachModelsUi ui)
        {
            Document document;
            BasicFileInfo fileInfo;
            bool isWorkshared = true;
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
                    ModelPath modelPath = new FilePath(filePath);
                    WorksetConfiguration worksetConfiguration = new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets);
                    document = OpenDocument.OpenDetached(application, modelPath, worksetConfiguration);
                    isWorkshared = true;
                }
            }
            catch (Exception ex)
            {
                return;
            }
            DeleteLinks(document);
            string fileDetachedPath = "";
            switch (ui.RadioButtonSavingPathMode)
            {
                case 1:
                    string folder = "";
                    ui.Dispatcher.Invoke(() => folder = @ui.TextBoxFolder.Text);
                    fileDetachedPath = folder + "\\" + document.Title.Replace("_detached", "").Replace("_отсоединено", "") + ".rvt";
                    break;
                case 3:
                    string maskIn = "";
                    string maskOut = "";
                    ui.Dispatcher.Invoke(() => maskIn = @ui.TextBoxMaskIn.Text);
                    ui.Dispatcher.Invoke(() => maskOut = @ui.TextBoxMaskOut.Text);
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

            ModelPath modelDetachedPath = new FilePath(fileDetachedPath);
            document?.SaveAs(modelDetachedPath, saveAsOptions);

            try
            {
                if (isWorkshared)
                    FreeTheModel(document);
            }
            catch
            {
            }

            document?.Close();
            document?.Dispose();
        }
        public static void UnloadRevitLinks(ModelPath location, bool isSameFolder, string folder)
        {
            TransmissionData transData = TransmissionData.ReadTransmissionData(location);

            if (transData != null)
            {
                ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();
                foreach (ElementId refId in externalReferences)
                {
                    ExternalFileReference extRef = transData.GetLastSavedReferenceData(refId);
                    if (extRef.ExternalFileReferenceType == ExternalFileReferenceType.RevitLink)
                    {
                        string name = extRef.GetPath().CentralServerPath.Split('\\').Last();
                        if (isSameFolder)
                        {
                            FilePath path = new FilePath(folder + '\\' + name);
                            transData.SetDesiredReferenceData(refId, path, PathType.Absolute, false);
                        }
                        else
                        {
                            transData.SetDesiredReferenceData(refId, extRef.GetPath(), extRef.PathType, false);
                        }
                    }
                }
                transData.IsTransmitted = true;
                TransmissionData.WriteTransmissionData(location, transData);
            }
            else
            {
                TaskDialog.Show("Unload Links", "The document does not have any transmission data");
            }
        }
        public static void ReplaceRevitLinks(ModelPath filePath, Dictionary<string, string> oldNewFilePairs)
        {
            TransmissionData transData = TransmissionData.ReadTransmissionData(filePath);

            if (transData != null)
            {
                ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();

                foreach (ElementId refId in externalReferences)
                {
                    ExternalFileReference extRef = transData.GetLastSavedReferenceData(refId);
                    ModelPath modelPath = extRef.GetAbsolutePath();
                    string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);
                    if (extRef.ExternalFileReferenceType == ExternalFileReferenceType.RevitLink && oldNewFilePairs.Any(e => e.Key == path))
                    {
                        string newFile = oldNewFilePairs.FirstOrDefault(e => e.Key == path).Value;
                        ModelPath newPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(newFile);
                        try
                        {
                            transData.SetDesiredReferenceData(refId, newPath, PathType.Absolute, true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            continue;
                        }
                    }
                }

                transData.IsTransmitted = true;
                TransmissionData.WriteTransmissionData(filePath, transData);
            }
            else
            {
                TaskDialog.Show("Replace Links", "The document does not have any transmission data");
            }
        }
        public static void FreeTheModel(Document document)
        {
            RelinquishOptions relinquishOptions = new(true);
            TransactWithCentralOptions transactWithCentralOptions = new();
            WorksharingUtils.RelinquishOwnership(document, relinquishOptions, transactWithCentralOptions);
        }
    }
}