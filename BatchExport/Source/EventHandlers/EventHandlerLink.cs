using Autodesk.Revit.UI;
using VLS.BatchExport.Utils;
using VLS.BatchExport.Views.Base;
using VLS.BatchExport.Views.Link;

namespace VLS.BatchExport.Source.EventHandlers
{
    public class EventHandlerLink : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (!(iConfigBase is LinkViewModel linkVM) || !linkVM.IsEverythingFilled()) return;

            linkVM.CreateLinks(uiApp);
            linkVM.Finisher(id: "LinkModelsFinished");
        }
    }
}