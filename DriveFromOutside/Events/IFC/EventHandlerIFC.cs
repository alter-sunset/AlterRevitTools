using Autodesk.Revit.UI;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.IFC;

namespace AlterTools.DriveFromOutside.Events.IFC
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