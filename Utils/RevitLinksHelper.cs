using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using VLS.BatchExportNet.Views.Link;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace VLS.BatchExportNet.Utils
{
    static class RevitLinksHelper
    {
        internal static void Delete(Document document)
        {
            using Transaction transaction = new(document);
            transaction.Start("Delete links from model");

            FailureHandlingOptions failOpt = transaction.GetFailureHandlingOptions();
            failOpt.SetFailuresPreprocessor(new CopyWatchAlertSwallower());
            transaction.SetFailureHandlingOptions(failOpt);

            List<Element> links = [.. new FilteredElementCollector(document).OfClass(typeof(RevitLinkType))];

            foreach (Element link in links)
            {
                document.Delete(link.Id);
            }
            transaction.Commit();
        }
        internal static void Unload(ModelPath location, bool isSameFolder, string folder)
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
                            FilePath path = new(folder + '\\' + name);
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
        internal static void Replace(ModelPath filePath, Dictionary<string, string> oldNewFilePairs)
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
        internal static void CreateLinks(UIApplication uiApp, LinkViewModel linkViewModel)
        {
            using Application application = uiApp.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            bool isCurrentWorkset = linkViewModel.IsCurrentWorkset;
            List<ListBoxItem> listItems = [.. linkViewModel.ListBoxItems];

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
            RevitLinkOptions options = new(false);

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
    }
}
