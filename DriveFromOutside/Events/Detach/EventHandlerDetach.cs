using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Detach;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;

namespace AlterTools.DriveFromOutside.Events.Detach;

public class EventHandlerDetach : RevitEventWrapper<IConfigDetach>
{
    protected override void Execute(UIApplication uiApp, IConfigDetach iConfigDetach)
    {
        using Application? app = uiApp.Application;

        string[] files = iConfigDetach.Files;
        foreach (string file in files)
        {
            using ErrorSuppressor errorSuppressor = new(uiApp);
            if (!File.Exists(file)) continue;
            iConfigDetach.DetachModel(app, file);
        }
    }
}