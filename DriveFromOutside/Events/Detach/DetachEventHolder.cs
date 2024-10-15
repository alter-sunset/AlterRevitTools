using Autodesk.Revit.UI;

namespace VLS.DriveFromOutside.Events.Detach
{
    public class DetachEventHolder : IEventHolder
    {
        private readonly EventHandlerDetach _eventHandlerDetach = new();
        public ExternalEvents ExternalEvent => ExternalEvents.Detach;
        public IExternalEventHandler ExternalEventHandler => _eventHandlerDetach;
    }
}