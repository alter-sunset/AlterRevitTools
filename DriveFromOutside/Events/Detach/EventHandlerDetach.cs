using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using AlterTools.BatchExportNet.Source.EventHandlers;
using AlterTools.BatchExportNet.Utils;
using AlterTools.BatchExportNet.Views.Detach;

namespace AlterTools.DriveFromOutside.Events.Detach
{
    public class EventHandlerDetach : RevitEventWrapper<IConfigDetach>
    {
        public override void Execute(UIApplication uiApp, IConfigDetach iConfigDetach)
        {
            using Application app = uiApp.Application;

            string[] files = iConfigDetach.Files;
            foreach (string file in files)
            {
                using ErrorSwallower errorSwallower = new(uiApp);
                if (!File.Exists(file)) continue;
                iConfigDetach.DetachModel(app, file);
            }
        }
    }
}