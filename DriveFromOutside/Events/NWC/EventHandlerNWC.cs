using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.NWC;
using Autodesk.Revit.UI;

namespace AlterTools.DriveFromOutside.Events.NWC;

public class EventHandlerNWC : RevitEventWrapper<NWCConfig>
{
    protected override void Execute(UIApplication uiApp, NWCConfig nwcConfig)
    {
        Logger log = new(nwcConfig.FolderPath);
        NWCHelper nwcHelper = new();
        nwcHelper.BatchExportModels(nwcConfig, uiApp, ref log);
        log.Dispose();
    }
}