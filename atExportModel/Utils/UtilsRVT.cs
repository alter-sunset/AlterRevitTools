using System.IO;
using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;
using AlterTools.Utils.Extensions;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Utils;

public static class UtilsRVT
{
    // TODO: fill it with stuff
    public static void CleanTheModel(IConfigClean config)
    {
    }

    public static void CleanupAndClose(Document doc, string fileDetachedPath, bool isWorkshared,
        RvtExportMode exportMode)
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

        // RevitServer path, no cleanup needed
        if (fileDetachedPath.StartsWith("RSN")) return;

        if (isWorkshared)
        {
            if (exportMode == RvtExportMode.Transmit)
            {
                using ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileDetachedPath);
                UpdateTransmissionData(modelPath);
            }

            string backupFolderPath = fileDetachedPath.Replace(".rvt", "_backup");
            if (!Directory.Exists(backupFolderPath)) return;

            Directory.Delete(backupFolderPath, true);
        }

        for (int i = 1; i <= 3; i++)
        {
            string versionedFilePath = fileDetachedPath.Replace(".rvt", $".{i:D4}.rvt");
            if (!File.Exists(versionedFilePath)) continue;

            File.Delete(versionedFilePath);
        }
    }

    private static void UpdateTransmissionData(ModelPath modelPath)
    {
        using TransmissionData transData = TransmissionData.ReadTransmissionData(modelPath);
        if (transData is null) return;

        transData.IsTransmitted = true;

        TransmissionData.WriteTransmissionData(modelPath, transData);
    }
}