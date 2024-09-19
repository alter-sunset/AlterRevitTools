using Autodesk.Revit.UI;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views;
using VLS.BatchExportNet.Views.Link;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerLinkModelsVMArg : EventHandlerBaseVMArgs
    {
        public override void Execute(UIApplication uiApp, ViewModelBase viewModelBase)
        {
            LinkViewModel linkViewModel = viewModelBase as LinkViewModel;
            if (!linkViewModel.IsEverythingFilled())
            {
                return;
            }
            linkViewModel.CreateLinks(uiApp);
            linkViewModel.Finisher("LinkModelsFinished");
        }
    }
}