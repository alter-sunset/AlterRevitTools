using Autodesk.Revit.UI;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;
using VLS.BatchExportNet.Views.NWC;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerNWC : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfig)
        {
            if (iConfig is not NWC_ViewModel nwcViewModel || !nwcViewModel.IsEverythingFilled()) return;

            Logger log = new(nwcViewModel.FolderPath);
            NWCHelper nwcHelper = new();

            nwcHelper.BatchExportModels(nwcViewModel, uiApp, ref log);

            int totalFiles = log.ErrorCount + log.SuccessCount;
            string msg = $"В процессе выполнения было {log.ErrorCount} ошибок из {totalFiles} файлов.";
            nwcViewModel.Finisher("ExportNWCFinished", msg);
        }
    }
}