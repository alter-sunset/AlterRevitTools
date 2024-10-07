using Autodesk.Revit.UI;

namespace VLS.DriveFromOutside.Events.NWC
{
    public class NWC_EventHolder : IEventHolder
    {
        private readonly EventHandlerNWC _eventHandlerNWC = new();
        private readonly ExternalEvents _externalEventNWC = ExternalEvents.NWC;
        public ExternalEvents ExternalEvent => _externalEventNWC;
        public IExternalEventHandler ExternalEventHandler => _eventHandlerNWC;
    }
}