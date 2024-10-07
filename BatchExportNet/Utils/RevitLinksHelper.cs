using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;

namespace VLS.BatchExportNet.Utils
{
    public static class RevitLinksHelper
    {
        private const string NO_TRANS_DATA_ALERT = "The document does not have any transmission data";

        public static void DeleteRevitLinks(this Document document)
        {
            using Transaction transaction = new(document);
            transaction.Start("Delete Revit links from model");

            transaction.SwallowAlert();

            List<Element> links = [.. new FilteredElementCollector(document).OfClass(typeof(RevitLinkType))];

            foreach (Element link in links)
            {
                document.Delete(link.Id);
            }
            transaction.Commit();
        }
        public static void UnloadRevitLinks(this ModelPath filePath, string folder, bool isSameFolder = true)
        {
            TransmissionData transData = TransmissionData.ReadTransmissionData(filePath);

            if (transData is null)
            {
                TaskDialog.Show("Unload Revit links", NO_TRANS_DATA_ALERT);
                return;
            }
            ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();
            foreach (ElementId refId in externalReferences)
            {
                ExternalFileReference extRef = transData.GetLastSavedReferenceData(refId);
                if (extRef.ExternalFileReferenceType is not ExternalFileReferenceType.RevitLink)
                    continue;

                string name = Path.GetFileName(extRef.GetPath().CentralServerPath);
                if (isSameFolder)
                {
                    FilePath path = new(Path.Combine(folder, name));
                    transData.SetDesiredReferenceData(refId, path, PathType.Absolute, false);
                }
                else
                {
                    transData.SetDesiredReferenceData(refId, extRef.GetPath(), extRef.PathType, false);
                }
            }
            transData.IsTransmitted = true;
            TransmissionData.WriteTransmissionData(filePath, transData);
        }
        public static void ReplaceLinks(this ModelPath filePath, Dictionary<string, string> oldNewFilePairs)
        {
            TransmissionData transData = TransmissionData.ReadTransmissionData(filePath);

            if (transData is null)
            {
                TaskDialog.Show("Replace Links", NO_TRANS_DATA_ALERT);
                return;
            }
            ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();
            foreach (ElementId refId in externalReferences)
            {
                ExternalFileReference extRef = transData.GetLastSavedReferenceData(refId);
                ModelPath modelPath = extRef.GetAbsolutePath();
                string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

                if (extRef.ExternalFileReferenceType is not ExternalFileReferenceType.RevitLink
                    || !oldNewFilePairs.Any(e => e.Key == path))
                    continue;

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
            transData.IsTransmitted = true;
            TransmissionData.WriteTransmissionData(filePath, transData);
        }
    }
}