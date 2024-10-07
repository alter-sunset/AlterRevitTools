using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VLS.BatchExportNet.Source;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.NWC;

namespace VLS.DriveFromOutside.Events.NWC
{
    public class EventHandlerNWC : RevitEventWrapper<NWC_Config>
    {
        public override void Execute(UIApplication uiApp, NWC_Config nwc_Config)
        {
            Logger logger = new(nwc_Config.FolderPath);
            NWCHelper nwcHelper = new();
            nwcHelper.BatchExportModels(nwc_Config, uiApp, ref logger);

            string msg = $"В процессе выполнения было {logger.ErrorCount} ошибок из {logger.ErrorCount + logger.SuccessCount} файлов.";
            logger.Dispose();
        }
    }
}
