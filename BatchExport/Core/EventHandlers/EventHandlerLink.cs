using Autodesk.Revit.UI;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Link;

namespace AlterTools.BatchExport.Core.EventHandlers
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