using AlterTools.DriveFromOutside.Events.Detach;
using AlterTools.DriveFromOutside.Events.IFC;
using AlterTools.DriveFromOutside.Events.NWC;
using AlterTools.DriveFromOutside.Events.Transmit;
using AlterTools.DriveFromOutside.Events;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.DriveFromOutside
{
    [Transaction(TransactionMode.Manual)]
    public class ExternalCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeListener();

            return Result.Succeeded;
        }

        private void InitializeListener()
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
            externalTaskHandler.LookForSingleTask(timeSpan);
        }
    }
}