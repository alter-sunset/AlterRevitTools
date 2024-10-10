using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using VLS.DriveFromOutside.Events;
using VLS.DriveFromOutside.Events.Detach;
using VLS.DriveFromOutside.Events.IFC;
using VLS.DriveFromOutside.Events.NWC;
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
            //Initialize all External Events
            List<IEventHolder> events =
            [
                new TransmitEventHolder(),
                new DetachEventHolder(),
                new NWC_EventHolder(),
                new IFC_EventHolder(),
            ];

            //Initialize Task Handler and pass Event instances to it
            ExternalTaskHandler externalTaskHandler = new(events);

            //Start listener, duh
            await externalTaskHandler.LookForSingleTask(TimeSpan.FromMinutes(1));
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.ApplicationInitialized -= OnInitialized;
            return Result.Succeeded;
        }
    }
}