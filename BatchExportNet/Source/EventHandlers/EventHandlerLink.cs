using Autodesk.Revit.UI;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;
using VLS.BatchExportNet.Views.Link;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerLink : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not LinkViewModel linkVM || !linkVM.IsEverythingFilled()) return;

            linkVM.CreateLinks(uiApp);
            linkVM.Finisher(id: "LinkModelsFinished");
        }
    }
}