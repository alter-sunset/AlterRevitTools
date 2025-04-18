using Autodesk.Revit.UI;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.IFC;

namespace AlterTools.BatchExport.Core.EventHandlers
{
    public class EventHandlerIfc : EventHandlerBase
    {
        protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not IfcViewModel ifcVm) return;
            if (!ifcVm.IsEverythingFilled()) return;

            Logger log = new(ifcVm.FolderPath);

            IfcHelper ifcHelper = new();
            ifcHelper.BatchExportModels(ifcVm, uiApp, ref log);

            string msg = $"В процессе выполнения было {log.ErrorCount} ошибок.";

            log.Dispose();

            ifcVm.Finisher(id: "ExportIFCFinished", msg);
        }
    }
}