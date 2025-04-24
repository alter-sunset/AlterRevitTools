using AlterTools.BatchExport.Utils;
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

        Logger log = new(ifcVm.FolderPath);

        IFCHelper ifcHelper = new();
        ifcHelper.BatchExportModels(ifcVm, uiApp, ref log);

        string msg = $"В процессе выполнения было {log.ErrorCount} ошибок.";

        log.Dispose();

        ifcVm.Finisher("ExportIFCFinished", msg);
    }
}