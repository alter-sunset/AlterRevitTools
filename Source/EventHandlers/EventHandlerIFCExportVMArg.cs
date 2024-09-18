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
            if (!ViewModelHelper.IsEverythingFilled(ifc_ViewModel))
            {
                return;
            }
            Logger logger = new(ifc_ViewModel.FolderPath);
            IFCHelper.BatchExportModels(uiApp, ifc_ViewModel, ref logger);

            string msg = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов.";
            logger.Dispose();
            ModelHelper.Finisher(ifc_ViewModel, "ExportIFCFinished", msg);
        }
    }
}