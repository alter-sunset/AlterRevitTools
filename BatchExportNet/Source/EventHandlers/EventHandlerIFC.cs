﻿using Autodesk.Revit.UI;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;
using VLS.BatchExportNet.Views.IFC;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerIFC : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfig)
        {
            IFC_ViewModel ifc_ViewModel = iConfig as IFC_ViewModel;
            if (!ifc_ViewModel.IsEverythingFilled()) return;

            Logger logger = new(ifc_ViewModel.FolderPath);
            IFCHelper ifcHelper = new();
            ifcHelper.BatchExportModels(ifc_ViewModel, uiApp, ref logger);

            string msg = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов.";
            ifc_ViewModel.Finisher(id: "ExportIFCFinished", msg);
        }
    }
}