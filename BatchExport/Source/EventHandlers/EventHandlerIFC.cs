﻿using Autodesk.Revit.UI;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.IFC;

namespace AlterTools.BatchExport.Source.EventHandlers
{
    public class EventHandlerIFC : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (!(iConfigBase is IFC_ViewModel ifcVM) || !ifcVM.IsEverythingFilled()) return;

            Logger log = new Logger(ifcVM.FolderPath);
            IFCHelper ifcHelper = new IFCHelper();
            ifcHelper.BatchExportModels(ifcVM, uiApp, ref log);

            string msg = $"В процессе выполнения было {log.ErrorCount} ошибок из {log.ErrorCount + log.SuccessCount} файлов.";
            ifcVM.Finisher(id: "ExportIFCFinished", msg);
        }
    }
}