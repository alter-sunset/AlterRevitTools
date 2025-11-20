using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Link;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerLink : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
    {
        if (iConfigBase is not LinkViewModel linkVm) return;

        if (!linkVm.IsEverythingFilled()) return;

        linkVm.CreateLinks(uiApp);

        linkVm.FinishWork("LinkModelsFinished");
    }
}