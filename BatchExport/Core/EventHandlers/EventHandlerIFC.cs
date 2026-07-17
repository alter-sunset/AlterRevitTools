using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.IFC;
using AlterTools.Resources;
using AlterTools.Utils.Logger;
using AlterTools.Utils.MVVM;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerIFC : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, ViewModelBase iConfigBase)
    {
        if (iConfigBase is not IFCViewModel ifcVm) return;
        if (!ifcVm.IsEverythingFilled()) return;

        ILogger log = LoggerFactory.CreateLogger(ifcVm.FolderPath, ifcVm.TurnOffLog);

        IFCHelper ifcHelper = new();
        ifcHelper.BatchExportModels(ifcVm, uiApp, ref log);

        string msg = $"{Strings.ThereWhere} {log.ErrorCount} {Strings.Errors}";

        log.Dispose();

        ifcVm.FinishWork("ExportIFCFinished", msg);
    }
}