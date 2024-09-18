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
            RevitLinksHelper.CreateLinks(uiApp, linkViewModel);
            ModelHelper.Finisher(linkViewModel, "LinkModelsFinished");
        }
    }
}