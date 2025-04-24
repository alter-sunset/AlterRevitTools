using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Utils;

public static class RevitLinksHelper
{
    private const string NoTransDataAlert = "The document doesn't have any transmission data";

    public static void UnloadRevitLinks(this ModelPath filePath, string folder, bool isSameFolder = true)
    {
        if (!TryGetTransmissionData(filePath, out TransmissionData transData)) return;

        ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();

        foreach (ElementId refId in externalReferences)
        {
            ExternalFileReference extRef = transData.GetLastSavedReferenceData(refId);
            if (ExternalFileReferenceType.RevitLink != extRef.ExternalFileReferenceType) continue;

            string name = Path.GetFileName(extRef.GetPath().CentralServerPath);
            if (null == name) continue;

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
            if (ExternalFileReferenceType.RevitLink != extRef.ExternalFileReferenceType) continue;

            ModelPath modelPath = extRef.GetAbsolutePath();
            string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

            if (!oldNewFilePairs.TryGetValue(path, out string newFile)) continue;

            ModelPath newPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(newFile);

            try
            {
                transData.SetDesiredReferenceData(refId, newPath, PathType.Absolute, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        transData.IsTransmitted = true;

        TransmissionData.WriteTransmissionData(filePath, transData);
    }

    private static bool TryGetTransmissionData(ModelPath filePath, out TransmissionData transData)
    {
        transData = TransmissionData.ReadTransmissionData(filePath);

        if (null != transData) return true;

        TaskDialog.Show("Operation Error", NoTransDataAlert);

        return false;
    }
}