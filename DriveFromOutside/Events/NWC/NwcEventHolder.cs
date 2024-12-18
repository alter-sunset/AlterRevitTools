using Autodesk.Revit.UI;

namespace AlterTools.DriveFromOutside.Events.NWC
{
    public class NwcEventHolder : IEventHolder
    {
        private readonly EventHandlerNWC _eventHandlerNWC = new();
        public ExternalEvents ExternalEvent => ExternalEvents.NWC;
        public IExternalEventHandler ExternalEventHandler => _eventHandlerNWC;
    }
}