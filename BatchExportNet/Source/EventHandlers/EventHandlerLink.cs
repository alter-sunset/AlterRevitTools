using Autodesk.Revit.UI;
using AlterTools.BatchExportNet.Utils;
using AlterTools.BatchExportNet.Views.Base;
using AlterTools.BatchExportNet.Views.Link;

namespace AlterTools.BatchExportNet.Source.EventHandlers
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