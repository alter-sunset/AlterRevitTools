using Autodesk.Revit.UI;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.NWC;

namespace VLS.DriveFromOutside.Events.NWC
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