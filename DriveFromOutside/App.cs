using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using AlterTools.DriveFromOutside.Events;
using AlterTools.DriveFromOutside.Events.Detach;
using AlterTools.DriveFromOutside.Events.IFC;
using AlterTools.DriveFromOutside.Events.NWC;
using AlterTools.DriveFromOutside.Events.Transmit;

namespace AlterTools.DriveFromOutside
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class App : IExternalApplication
    {//TODO: add support for building and running .cs scripts into tasks
        public Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.ApplicationInitialized += OnInitialized;
            return Result.Succeeded;
        }

        private async void OnInitialized(object? sender, ApplicationInitializedEventArgs e)
        {
            //Initialize all External Events
            List<IEventHolder> events =
            [
                new TransmitEventHolder(),
                new DetachEventHolder(),
                new NwcEventHolder(),
                new IfcEventHolder(),
            ];

            //Initialize Task Handler and pass Event instances to it
            ExternalTaskHandler externalTaskHandler = new(events);

            //Start listener, duh
            TimeSpan timeSpan = TimeSpan.FromMinutes(1);

#if R25_OR_GREATER
            await externalTaskHandler.LookForSingleTask(timeSpan);
#else
            externalTaskHandler.LookForSingleTask(timeSpan);
#endif
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.ApplicationInitialized -= OnInitialized;
            return Result.Succeeded;
        }
    }
}