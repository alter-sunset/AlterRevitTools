using Autodesk.Revit.UI;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;
using VLS.BatchExportNet.Views.IFC;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerIFC : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfig)
        {
            if (iConfig is not IFC_ViewModel ifcViewModel || !ifcViewModel.IsEverythingFilled()) return;

            Logger logger = new(ifcViewModel.FolderPath);
            IFCHelper ifcHelper = new();
            ifcHelper.BatchExportModels(ifcViewModel, uiApp, ref logger);

            string msg = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов.";
            ifcViewModel.Finisher(id: "ExportIFCFinished", msg);
        }
    }
}