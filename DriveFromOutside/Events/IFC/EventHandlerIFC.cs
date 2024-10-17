using Autodesk.Revit.UI;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.IFC;

namespace VLS.DriveFromOutside.Events.IFC
{
    public class EventHandlerIFC : RevitEventWrapper<IFC_Config>
    {
        public override void Execute(UIApplication uiApp, IFC_Config ifc_Config)
        {
            Logger logger = new(ifc_Config.FolderPath);
            IFCHelper ifcHelper = new();
            ifcHelper.BatchExportModels(ifc_Config, uiApp, ref logger);
        }
    }
}