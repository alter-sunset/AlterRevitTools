using Autodesk.Revit.UI;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;
using VLS.BatchExportNet.Views.NWC;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerNWCExportVMArg : EventHandlerBaseVMArgs
    {
        public override void Execute(UIApplication uiApp, ViewModelBase viewModelBase)
        {
            NWC_ViewModel nwc_ViewModel = viewModelBase as NWC_ViewModel;
            if (!nwc_ViewModel.IsEverythingFilled())
            {
                return;
            }
            Logger logger = new(nwc_ViewModel.FolderPath);
            NWCHelper nwcHelper = new();
            nwcHelper.BatchExportModels(nwc_ViewModel, uiApp, ref logger);

            string msg = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов.";
            logger.Dispose();
            nwc_ViewModel.Finisher("ExportNWCFinished", msg);
        }
    }
}