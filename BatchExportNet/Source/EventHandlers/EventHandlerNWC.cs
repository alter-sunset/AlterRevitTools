using Autodesk.Revit.UI;
using AlterTools.BatchExportNet.Utils;
using AlterTools.BatchExportNet.Views.Base;
using AlterTools.BatchExportNet.Views.NWC;

namespace AlterTools.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerNWC : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not NWC_ViewModel nwcVM || !nwcVM.IsEverythingFilled()) return;

            Logger log = new(nwcVM.FolderPath);
            NWCHelper nwcHelper = new();

            nwcHelper.BatchExportModels(nwcVM, uiApp, ref log);

            int totalFiles = log.ErrorCount + log.SuccessCount;
            string msg = $"В процессе выполнения было {log.ErrorCount} ошибок из {totalFiles} файлов.";
            nwcVM.Finisher("ExportNWCFinished", msg);
        }
    }
}