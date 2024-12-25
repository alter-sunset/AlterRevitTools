using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AlterTools.BatchExport.Source.EventHandlers;
using AlterTools.BatchExport.Utils;

namespace AlterTools.DriveFromOutside.Events.Transmit
{
    public class EventHandlerTransmit : RevitEventWrapper<TransmitConfig>
    {
        public override void Execute(UIApplication uiApp, TransmitConfig transmitConfig)
        {
            using Application app = uiApp.Application;

            foreach (string file in transmitConfig.Files)
            {
                if (!File.Exists(file)) continue;

                string folder = transmitConfig.FolderPath;

                string transmittedFilePath = Path.Combine(folder, Path.GetFileName(file));
                File.Copy(file, transmittedFilePath, true);
                ModelPath transmittedModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(transmittedFilePath);
                transmittedModelPath.UnloadRevitLinks(folder);
            }
        }
    }
}