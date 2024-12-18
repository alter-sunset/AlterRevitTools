using Autodesk.Revit.UI;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.NWC;

namespace AlterTools.BatchExport.Source.EventHandlers
{
    public class EventHandlerNWC : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (!(iConfigBase is NWC_ViewModel nwcVM) || !nwcVM.IsEverythingFilled()) return;

            Logger log = new Logger(nwcVM.FolderPath);
            NWCHelper nwcHelper = new NWCHelper();

            nwcHelper.BatchExportModels(nwcVM, uiApp, ref log);

            int totalFiles = log.ErrorCount + log.SuccessCount;
            string msg = $"В процессе выполнения было {log.ErrorCount} ошибок из {totalFiles} файлов.";
            nwcVM.Finisher("ExportNWCFinished", msg);
        }
    }
}