using Autodesk.Revit.UI;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.NWC;

namespace VLS.DriveFromOutside.Events.NWC
{
    public class EventHandlerNWC : RevitEventWrapper<NWC_Config>
    {
        public override void Execute(UIApplication uiApp, NWC_Config nwc_Config)
        {
            Logger logger = new(nwc_Config.FolderPath);
            NWCHelper nwcHelper = new();
            nwcHelper.BatchExportModels(nwc_Config, uiApp, ref logger);
        }
    }
}