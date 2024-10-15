using Autodesk.Revit.UI;

namespace VLS.DriveFromOutside.Events.IFC
{
    public class IFC_EventHolder : IEventHolder
    {
        private readonly EventHandlerIFC _eventHandlerIFC = new();
        public ExternalEvents ExternalEvent => ExternalEvents.IFC;
        public IExternalEventHandler ExternalEventHandler => _eventHandlerIFC;
    }
}