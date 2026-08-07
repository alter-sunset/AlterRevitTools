using System.IO;
using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Enums;
using AlterTools.Utils.Extensions;
using Autodesk.Revit.DB;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.atExportModel.Utils;

public static class UtilsMain
{
    public static void ProcessModel(Application app, ConfigExportSingle config)
    {
        bool dontOpen = !config.ExportNWC &&
                        !config.ExportIFC &&
                        !config.CleanModel &&
                        config.ExportRVT &&
                        config.RvtExportMode == RvtExportMode.Transmit;

        // 1. Check if file should be opened at all -- forgot about RevitServer, maybe need to add external worker?
        if (dontOpen)
        {
            string transFilePath = Path.Combine(config.FolderPathRVT, Path.GetFileName(config.FileName));
            File.Copy(config.FileName, transFilePath, true);

            // TODO: add copying mechanism for RS

            using ModelPath transModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(transFilePath);
            transModelPath.UnloadRevitLinks(config.FolderPathRVT);
            return;
        }

        // Open model
        using Document doc = DocumentExtensions.OpenDocument(config.FileName, app, out bool isWorkshared);
        if (doc is null) return;

        string modelName = doc.Title.RemoveDetach();

        // Export NWC
        if (config.ExportNWC)
        {
            using NavisworksExportOptions options = UtilsNWC.GetNWCExportOptions(config, doc);
            doc.Export(config.FolderPathNWC, modelName, options);
        }

        // Export ifc and rollback transaction
        if (config.ExportIFC)
        {
            using Transaction tr = new(doc);
            tr.Start(Resources.Strings.IFCTitle);

            using IFCExportOptions options = UtilsIFC.GetIFCExportOptions(config, doc);
            doc.Export(config.FolderPathIFC, modelName, options);

            tr.RollBack();
        }

        if (config.ExportRVT)
        {
            // Clean
            if (config.CleanModel)
            {
                UtilsRVT.CleanTheModel(config.ConfigClean);
            }

            string fileDetachedPath = Path.Combine(config.FolderPathRVT, $"{modelName}.rvt");

            using ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(config.FileName);
            TransmissionData transData = null;
            if (isWorkshared) transData = TransmissionData.ReadTransmissionData(modelPath);

            DocumentExtensions.SaveDocument(doc, fileDetachedPath, isWorkshared, transData);

            UtilsRVT.CleanupAndClose(doc, fileDetachedPath, isWorkshared, config.RvtExportMode);
            return;
        }

        DocumentExtensions.CloseDocument(doc);
    }
}