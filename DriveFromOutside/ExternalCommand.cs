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
            Task.Run(async () =>
            {
                await SignalRService.Instance.StartAsync("https://localhost:5050/RevitHub");
            }).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    TaskDialog.Show("Error", $"SignalR Connection Failed: {t.Exception}");
                }
            });

            ////Initialize all External Events
            //List<IEventHolder> events =
            //[
            //    new TransmitEventHolder(),
            //    new DetachEventHolder(),
            //    new NwcEventHolder(),
            //    new IfcEventHolder(),
            //];

            ////Initialize Task Handler and pass Event instances to it
            //ExternalTaskHandler externalTaskHandler = new(events);

            ////Start listener, duh
            //TimeSpan timeSpan = TimeSpan.FromMinutes(1);
            //externalTaskHandler.LookForSingleTask(timeSpan);
        }
    }
}