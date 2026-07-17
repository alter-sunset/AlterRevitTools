using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.NWC;
using AlterTools.Resources;
using AlterTools.Utils.Logger;
using AlterTools.Utils.MVVM;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerNWC : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, ViewModelBase iConfigBase)
    {
        if (iConfigBase is not NWCViewModel nwcVm) return;
        if (!nwcVm.IsEverythingFilled()) return;

        ILogger log = LoggerFactory.CreateLogger(nwcVm.FolderPath, nwcVm.TurnOffLog);

        NWCHelper nwcHelper = new();
        nwcHelper.BatchExportModels(nwcVm, uiApp, ref log);

        string msg = $"{Strings.ThereWhere} {log.ErrorCount} {Strings.Errors}";

        log.Dispose();

        nwcVm.FinishWork("ExportNWCFinished", msg);
    }
}