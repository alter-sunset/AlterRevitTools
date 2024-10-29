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
            if (viewModelBase is not LinkViewModel linkViewModel || !linkViewModel.IsEverythingFilled()) return;

            linkViewModel.CreateLinks(uiApp);
            linkViewModel.Finisher(id: "LinkModelsFinished");
        }
    }
}