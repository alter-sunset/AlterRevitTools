using Autodesk.Revit.UI;

namespace VLS.DriveFromOutside.Events.IFC
{
    public class IFC_EventHolder : IEventHolder
    {
        private readonly EventHandlerIFC _eventHandlerIFC = new();
        private readonly ExternalEvents _externalEventIFC = ExternalEvents.IFC;
        public ExternalEvents ExternalEvent => _externalEventIFC;
        public IExternalEventHandler ExternalEventHandler => _eventHandlerIFC;
    }
}