using Autodesk.Revit.UI;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.NWC;

namespace AlterTools.BatchExport.Core.EventHandlers
{
    public class EventHandlerNwc : EventHandlerBase
    {
        protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not NwcViewModel nwcVm) return;
            if (!nwcVm.IsEverythingFilled()) return;

            Logger log = new(nwcVm.FolderPath);

            NwcHelper nwcHelper = new();
            nwcHelper.BatchExportModels(nwcVm, uiApp, ref log);

            string msg = $"В процессе выполнения было {log.ErrorCount} ошибок.";

            log.Dispose();

            nwcVm.Finisher("ExportNWCFinished", msg);
        }
    }
}