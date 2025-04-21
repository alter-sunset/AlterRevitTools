using AlterTools.DriveFromOutside.Events;
using AlterTools.DriveFromOutside.Events.Detach;
using AlterTools.DriveFromOutside.Events.IFC;
using AlterTools.DriveFromOutside.Events.NWC;
using AlterTools.DriveFromOutside.Events.Transmit;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace AlterTools.DriveFromOutside;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
internal class App : IExternalApplication
{
    //TODO: add support for building and running .cs scripts into tasks
    public Result OnStartup(UIControlledApplication app)
    {
        app.ControlledApplication.ApplicationInitialized += OnInitialized;
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication app)
    {
        app.ControlledApplication.ApplicationInitialized -= OnInitialized;
        return Result.Succeeded;
    }

    private static async void OnInitialized(object? sender, ApplicationInitializedEventArgs e)
    {
        //Initialize all External Events
        List<IEventHolder> events =
        [
            new TransmitEventHolder(),
            new DetachEventHolder(),
            new NWCEventHolder(),
            new IFCEventHolder()
        ];

        //Initialize Task Handler and pass Event instances to it
        ExternalTaskHandler externalTaskHandler = new(events);

        //Start listener, duh
        var timeSpan = TimeSpan.FromMinutes(1);

#if R25_OR_GREATER
            await externalTaskHandler.LookForSingleTask(timeSpan);
#else
        externalTaskHandler.LookForSingleTask(timeSpan);
#endif
    }
}