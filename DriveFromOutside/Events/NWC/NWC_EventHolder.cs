using Autodesk.Revit.UI;

namespace VLS.DriveFromOutside.Events.NWC
{
    public class NWC_EventHolder : IEventHolder
    {
        private readonly EventHandlerNWC _eventHandlerNWC = new();
        public ExternalEvents ExternalEvent => ExternalEvents.NWC;
        public IExternalEventHandler ExternalEventHandler => _eventHandlerNWC;
    }
}