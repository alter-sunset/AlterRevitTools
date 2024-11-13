using Autodesk.Revit.UI;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.IFC;

namespace VLS.DriveFromOutside.Events.IFC
{
    public class EventHandlerIFC : RevitEventWrapper<IfcConfig>
    {
        public override void Execute(UIApplication uiApp, IfcConfig ifcConfig)
        {
            Logger log = new(ifcConfig.FolderPath);
            IFCHelper ifcHelper = new();
            ifcHelper.BatchExportModels(ifcConfig, uiApp, ref log);
        }
    }
}