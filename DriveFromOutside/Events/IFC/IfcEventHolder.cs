using Autodesk.Revit.UI;

namespace AlterTools.DriveFromOutside.Events.IFC
{
    public class IfcEventHolder : IEventHolder
    {
        private readonly EventHandlerIFC _eventHandlerIFC = new();
        public ExternalEvents ExternalEvent => ExternalEvents.IFC;
        public IExternalEventHandler ExternalEventHandler => _eventHandlerIFC;
    }
}