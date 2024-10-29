using Autodesk.Revit.UI;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;
using VLS.BatchExportNet.Views.Link;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerLink : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase viewModelBase)
        {
            LinkViewModel linkViewModel = viewModelBase as LinkViewModel;
            if (!linkViewModel.IsEverythingFilled()) return;
            linkViewModel.IsViewEnabled = false;
            linkViewModel.CreateLinks(uiApp);
            linkViewModel.Finisher(id: "LinkModelsFinished");
            linkViewModel.IsViewEnabled = true;
        }
    }
}