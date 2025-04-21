using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.DriveFromOutside.Events.Transmit;

public class EventHandlerTransmit : RevitEventWrapper<TransmitConfig>
{
    protected override void Execute(UIApplication uiApp, TransmitConfig transmitConfig)
    {
        using var app = uiApp.Application;

        foreach (var file in transmitConfig.Files)
        {
            if (!File.Exists(file)) continue;

            var folder = transmitConfig.FolderPath;

            var transmittedFilePath = Path.Combine(folder, Path.GetFileName(file));
            File.Copy(file, transmittedFilePath, true);
            var transmittedModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(transmittedFilePath);
            transmittedModelPath.UnloadRevitLinks(folder);
        }
    }
}