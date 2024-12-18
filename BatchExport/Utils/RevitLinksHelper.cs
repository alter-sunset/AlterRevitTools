using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;

namespace AlterTools.BatchExport.Utils
{
    public static class RevitLinksHelper
    {
        private const string NO_TRANS_DATA_ALERT = "The document doesn't have any transmission data";
        public static void UnloadRevitLinks(this ModelPath filePath, string folder, bool isSameFolder = true)
        {
            if (!TryGetTransmissionData(filePath, out TransmissionData transData)) return;

            ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();
            foreach (ElementId refId in externalReferences)
            {
                ExternalFileReference extRef = transData.GetLastSavedReferenceData(refId);
                if (!(extRef.ExternalFileReferenceType is ExternalFileReferenceType.RevitLink)) continue;

                string name = Path.GetFileName(extRef.GetPath().CentralServerPath);

                (ModelPath path, PathType pathType) = isSameFolder
                    ? (new FilePath(Path.Combine(folder, name)), PathType.Absolute)
                    : (extRef.GetPath(), extRef.PathType);

                transData.SetDesiredReferenceData(refId, path, pathType, false);
            }
            transData.IsTransmitted = true;
            TransmissionData.WriteTransmissionData(filePath, transData);
        }
        public static void ReplaceLinks(this ModelPath filePath, Dictionary<string, string> oldNewFilePairs)
        {
            if (!TryGetTransmissionData(filePath, out TransmissionData transData)) return;

            ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();
            foreach (ElementId refId in externalReferences)
            {
                ExternalFileReference extRef = transData.GetLastSavedReferenceData(refId);
                if (extRef.ExternalFileReferenceType != ExternalFileReferenceType.RevitLink) continue;

                ModelPath modelPath = extRef.GetAbsolutePath();
                string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);
                if (!oldNewFilePairs.TryGetValue(path, out var newFile)) continue;

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
        private static bool TryGetTransmissionData(ModelPath filePath, out TransmissionData transData)
        {
            transData = TransmissionData.ReadTransmissionData(filePath);
            if (!(transData is null)) return true;

            TaskDialog.Show("Operation Error", NO_TRANS_DATA_ALERT);
            return false;
        }
    }
}