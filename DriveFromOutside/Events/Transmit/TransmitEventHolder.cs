using Autodesk.Revit.UI;
using VLS.DriveFromOutside.Events;

namespace VLS.DriveFromOutside.Events.Transmit
{
    /// <summary>
    /// Class that will hold Transmit Event
    /// </summary>
    public class TransmitEventHolder : IEventHolder
    {
        private readonly EventHandlerTransmit _eventHandlerTransmit = new();
        private readonly ExternalEvents _externalEventTransmit = ExternalEvents.Transmit;
        public ExternalEvents ExternalEvent { get => _externalEventTransmit; }
        public IExternalEventHandler ExternalEventHandler { get => _eventHandlerTransmit; }
    }
}