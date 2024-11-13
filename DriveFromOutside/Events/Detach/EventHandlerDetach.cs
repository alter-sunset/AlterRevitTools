using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Detach;

namespace VLS.DriveFromOutside.Events.Detach
{
    public class EventHandlerDetach : RevitEventWrapper<IConfigDetach>
    {
        public override void Execute(UIApplication uiApp, IConfigDetach iDetachConfig)
        {
            using Application app = uiApp.Application;

            string[] files = iDetachConfig.Files;
            foreach (string file in files)
            {
                using ErrorSwallower errorSwallower = new(uiApp, app);
                if (!File.Exists(file)) continue;
                iDetachConfig.DetachModel(app, file);
            }
        }
    }
}