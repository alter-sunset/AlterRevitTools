using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Utils.Logger;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.NWC;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerNWC : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
    {
        if (iConfigBase is not NWCViewModel nwcVm) return;

        if (!nwcVm.IsEverythingFilled()) return;

        ILogger log = LoggerFactory.CreateLogger(nwcVm.FolderPath, nwcVm.TurnOffLog);

        NWCHelper nwcHelper = new();
        nwcHelper.BatchExportModels(nwcVm, uiApp, ref log);

        string msg = $"В процессе выполнения было {log.ErrorCount} ошибок.";

        log.Dispose();

        nwcVm.Finisher("ExportNWCFinished", msg);
    }
}