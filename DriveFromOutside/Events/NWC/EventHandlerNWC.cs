using Autodesk.Revit.UI;
using AlterTools.BatchExportNet.Source.EventHandlers;
using AlterTools.BatchExportNet.Utils;
using AlterTools.BatchExportNet.Views.NWC;

namespace AlterTools.DriveFromOutside.Events.NWC
{
    public class EventHandlerNWC : RevitEventWrapper<NwcConfig>
    {
        public override void Execute(UIApplication uiApp, NwcConfig nwcConfig)
        {
            Logger log = new(nwcConfig.FolderPath);
            NWCHelper nwcHelper = new();
            nwcHelper.BatchExportModels(nwcConfig, uiApp, ref log);
        }
    }
}