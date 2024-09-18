using Autodesk.Revit.UI;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views;
using VLS.BatchExportNet.Views.NWC;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerNWCExportVMArg : EventHandlerBaseVMArgs
    {
        public override void Execute(UIApplication uiApp, ViewModelBase viewModelBase)
        {
            NWC_ViewModel nwc_ViewModel = viewModelBase as NWC_ViewModel;
            if (!ViewModelHelper.IsEverythingFilled(nwc_ViewModel))
            {
                return;
            }
            Logger logger = new(nwc_ViewModel.FolderPath);
            NWCHelper.BatchExportModels(uiApp, nwc_ViewModel, ref logger);

            string msg = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов.";
            logger.Dispose();
            ModelHelper.Finisher(nwc_ViewModel, "ExportNWCFinished", msg);
        }
    }
}