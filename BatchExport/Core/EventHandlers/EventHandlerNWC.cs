using Autodesk.Revit.UI;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.NWC;

namespace AlterTools.BatchExport.Core.EventHandlers
{
    public class EventHandlerNWC : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not NWC_ViewModel nwcVM) return;
            if (!nwcVM.IsEverythingFilled()) return;

            Logger log = new(nwcVM.FolderPath);

            NWCHelper nwcHelper = new();
            nwcHelper.BatchExportModels(nwcVM, uiApp, ref log);

            string msg = $"В процессе выполнения было {log.ErrorCount} ошибок из {log.ErrorCount + log.SuccessCount} файлов.";

            log.Dispose();

            nwcVM.Finisher("ExportNWCFinished", msg);
        }
    }
}