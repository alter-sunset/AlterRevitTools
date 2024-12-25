﻿using Autodesk.Revit.UI;
using AlterTools.BatchExport.Source.EventHandlers;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.NWC;

namespace AlterTools.DriveFromOutside.Events.NWC
{
    public class EventHandlerNWC : RevitEventWrapper<NwcConfig>
    {
        public override void Execute(UIApplication uiApp, NwcConfig nwcConfig)
        {
            Logger log = new(nwcConfig.FolderPath);
            NWCHelper nwcHelper = new();
            nwcHelper.BatchExportModels(nwcConfig, uiApp, ref log);
        }
    }
}