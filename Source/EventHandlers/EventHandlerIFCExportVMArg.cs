using Autodesk.Revit.UI;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views;
using VLS.BatchExportNet.Views.IFC;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerIFCExportVMArg : EventHandlerBaseVMArgs
    {
        public override void Execute(UIApplication uiApp, ViewModelBase viewModelBase)
        {
            IFC_ViewModel ifc_ViewModel = viewModelBase as IFC_ViewModel;
            if (!ifc_ViewModel.IsEverythingFilled())
            {
                return;
            }
            Logger logger = new(ifc_ViewModel.FolderPath);
            ifc_ViewModel.BatchExportModels(uiApp, ref logger);

            string msg = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов.";
            logger.Dispose();
            ifc_ViewModel.Finisher("ExportIFCFinished", msg);
        }
    }
}