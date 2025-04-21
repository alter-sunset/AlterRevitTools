using Autodesk.Revit.UI;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.IFC;

namespace AlterTools.DriveFromOutside.Events.IFC
{
    public class EventHandlerIFC : RevitEventWrapper<IFCConfig>
    {
        protected override void Execute(UIApplication uiApp, IFCConfig ifcConfig)
        {
            Logger log = new(ifcConfig.FolderPath);
            IFCHelper ifcHelper = new();
            ifcHelper.BatchExportModels(ifcConfig, uiApp, ref log);
            log.Dispose();
        }
    }
}