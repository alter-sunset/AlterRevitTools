using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Utils.Logger;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.IFC;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerIFC : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
    {
        if (iConfigBase is not IFCViewModel ifcVm) return;

        if (!ifcVm.IsEverythingFilled()) return;

        ILogger log = LoggerFactory.CreateLogger(ifcVm.FolderPath, ifcVm.TurnOffLog);

        IFCHelper ifcHelper = new();
        ifcHelper.BatchExportModels(ifcVm, uiApp, ref log);

        string msg = $"{Resources.Resources.Const_ThereWhere} {log.ErrorCount} {Resources.Resources.Const_Errors}";

        log.Dispose();

        ifcVm.Finisher("ExportIFCFinished", msg);
    }
}