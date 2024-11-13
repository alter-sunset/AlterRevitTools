using Autodesk.Revit.UI;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;
using VLS.BatchExportNet.Views.IFC;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerIFC : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not IFC_ViewModel ifcVM || !ifcVM.IsEverythingFilled()) return;

            Logger log = new(ifcVM.FolderPath);
            IFCHelper ifcHelper = new();
            ifcHelper.BatchExportModels(ifcVM, uiApp, ref log);

            string msg = $"В процессе выполнения было {log.ErrorCount} ошибок из {log.ErrorCount + log.SuccessCount} файлов.";
            ifcVM.Finisher(id: "ExportIFCFinished", msg);
        }
    }
}