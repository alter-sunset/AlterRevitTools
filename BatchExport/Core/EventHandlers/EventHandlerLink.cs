using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Link;
using AlterTools.Utils.MVVM;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerLink : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, ViewModelBase iConfigBase)
    {
        if (iConfigBase is not LinkViewModel linkVm) return;
        if (!linkVm.IsEverythingFilled()) return;

        linkVm.CreateLinks(uiApp);

        linkVm.FinishWork("LinkModelsFinished");
    }
}