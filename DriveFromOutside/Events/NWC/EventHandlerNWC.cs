using Autodesk.Revit.UI;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.NWC;

namespace AlterTools.DriveFromOutside.Events.NWC
{
    public class EventHandlerNWC : RevitEventWrapper<NwcConfig>
    {
        protected override void Execute(UIApplication uiApp, NwcConfig nwcConfig)
        {
            Logger log = new(nwcConfig.FolderPath);
            NwcHelper nwcHelper = new();
            nwcHelper.BatchExportModels(nwcConfig, uiApp, ref log);
            log.Dispose();
        }
    }
}