using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace VLS.BatchExportNet.Utils
{
    static class RevitLinksHelper
    {
        internal static void DeleteRevitLinks(this Document document)
        {
            using Transaction transaction = new(document);
            transaction.Start("Delete Revit links from model");

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
        internal static void UnloadRevitLinks(this ModelPath filePath, bool isSameFolder, string folder)
        {
            TransmissionData transData = TransmissionData.ReadTransmissionData(filePath);

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
                TransmissionData.WriteTransmissionData(filePath, transData);
            }
            else
            {
                TaskDialog.Show("Unload Revit links", "The document does not have any transmission data");
            }
        }
        internal static void ReplaceLinks(this ModelPath filePath, Dictionary<string, string> oldNewFilePairs)
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
    }
}