using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using VLS.DriveFromOutside.Events;
using VLS.DriveFromOutside.Events.Detach;
using VLS.DriveFromOutside.Events.Transmit;

namespace VLS.DriveFromOutside
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
            app.ControlledApplication.ApplicationInitialized += OnInitialized;
            return Result.Succeeded;
        }

        private async void OnInitialized(object? sender, ApplicationInitializedEventArgs e)
        {
            Application? application = sender as Application;

            //Initialize all External Events
            List<IEventHolder> events = [];
            events.Add(new TransmitEventHolder());
            events.Add(new DetachEventHolder());

            //Initialize Task Handler and pass Event instances to it
            ExternalTaskHandler externalTaskHandler = new(application, events);

            //Start listener, duh
            await externalTaskHandler.ListenForNewTasks(TimeSpan.FromMinutes(1));
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.ApplicationInitialized -= OnInitialized;
            return Result.Succeeded;
        }
    }
}
