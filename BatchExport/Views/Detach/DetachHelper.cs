﻿using System;
using System.IO;
using System.Linq;
using AlterTools.BatchExport.Utils;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.Detach;

public static class DetachHelper
{
    public static void DetachModel(this IConfigDetach iConfigDetach, Application app, string filePath)
    {
        try
        {
            Document doc = app.OpenDocument(filePath, out bool isWorkshared);
            if (doc is null) return;

            string fileDetachedPath = GetDetachedFilePath(iConfigDetach, doc, filePath);

            ProcessDocument(doc, iConfigDetach);
            SaveDocument(doc, fileDetachedPath, isWorkshared);
            Cleanup(doc, fileDetachedPath, isWorkshared);
        }
        catch
        {
            // ignored
        }
    }

    private static string GetDetachedFilePath(IConfigDetach iConfigDetach, Document doc, string originalFilePath)
    {
        string docTitle = doc.Title.RemoveDetach();
        string fileDetachedPath = Path.Combine(iConfigDetach.FolderPath, $"{docTitle}.rvt");

        if (iConfigDetach is DetachViewModel { RadioButtonMode: 2 } detachViewModel)
            fileDetachedPath = RenamePath(originalFilePath,
                RenameType.Folder,
                detachViewModel.MaskIn, detachViewModel.MaskOut);

        if (iConfigDetach.IsToRename)
            fileDetachedPath = RenamePath(fileDetachedPath,
                RenameType.Title,
                iConfigDetach.MaskInName, iConfigDetach.MaskOutName);

        if (iConfigDetach.CheckForEmptyView) CheckAndModifyForEmptyView(doc, iConfigDetach, ref fileDetachedPath);

        return fileDetachedPath;
    }

    private static void CheckAndModifyForEmptyView(Document doc, IConfigDetach iConfigDetach,
        ref string fileDetachedPath)
    {
        doc.OpenAllWorksets();

        try
        {
            Element view = new FilteredElementCollector(doc)
                .OfClass(typeof(View3D))
                .FirstOrDefault(el => el.Name == iConfigDetach.ViewName && !((View3D)el).IsTemplate);

            if (view is not null
                && doc.IsViewEmpty(view))
                fileDetachedPath = RenamePath(fileDetachedPath, RenameType.Empty);
        }
        catch
        {
            // ignored
        }
    }

    private static string RenamePath(string filePath, RenameType renameType, string maskIn = "",
        string maskOut = "")
    {
        string title = Path.GetFileNameWithoutExtension(filePath);
        string folder = Path.GetDirectoryName(filePath);
        string extension = Path.GetExtension(filePath);

        switch (renameType)
        {
            case RenameType.Folder:
                folder = folder!.Replace(maskIn, maskOut);
                break;

            case RenameType.Title:
                title = title.Replace(maskIn, maskOut);
                break;

            case RenameType.Empty:
                title = $"EMPTY_{title}";
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(renameType), renameType, null);
        }

        return Path.Combine(folder!, $"{title}{extension}");
    }

    private static void ProcessDocument(Document doc, IConfigDetach iConfigDetach)
    {
        if (iConfigDetach.RemoveLinks) doc.DeleteAllLinks();

#if R22_OR_GREATER
        if (iConfigDetach.RemoveEmptyWorksets
            && doc.IsWorkshared)
            doc.RemoveEmptyWorksets();
#endif

#if R24_OR_GREATER
        if (iConfigDetach.Purge)
        {
            doc.PurgeAll();
        }
#endif
    }

    private static void SaveDocument(Document doc, string fileDetachedPath, bool isWorkshared)
    {
        SaveAsOptions saveOptions = new()
        {
            OverwriteExistingFile = true,
            MaximumBackups = 1
        };

        if (isWorkshared)
        {
            WorksharingSaveAsOptions worksharingOptions = new() { SaveAsCentral = true };
            saveOptions.SetWorksharingOptions(worksharingOptions);
        }

        try
        {
            ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileDetachedPath);
            doc.SaveAs(modelPath, saveOptions);
        }
        catch
        {
            // ignored
        }
    }

    private static void Cleanup(Document doc, string fileDetachedPath, bool isWorkshared)
    {
        try
        {
            doc.FreeTheModel();
        }
        catch
        {
            // ignored
        }
        finally
        {
            doc?.Close();
        }

        if (isWorkshared)
        {
            ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileDetachedPath);
            UpdateTransmissionData(modelPath);

            string backupFolderPath = fileDetachedPath.Replace(".rvt", "_backup");

            if (Directory.Exists(backupFolderPath)) Directory.Delete(backupFolderPath, true);

            return;
        }

        for (int i = 1; i <= 3; i++)
        {
            string versionedFilePath = fileDetachedPath.Replace(".rvt", $".{i:D4}.rvt");
            if (File.Exists(versionedFilePath)) File.Delete(versionedFilePath);
        }
    }

    private static void UpdateTransmissionData(ModelPath modelPath)
    {
        TransmissionData transData = TransmissionData.ReadTransmissionData(modelPath);
        if (transData is null) return;

        transData.IsTransmitted = true;

        TransmissionData.WriteTransmissionData(modelPath, transData);
    }
}