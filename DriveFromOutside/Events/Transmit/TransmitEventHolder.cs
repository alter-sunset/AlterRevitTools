using Autodesk.Revit.UI;

namespace AlterTools.DriveFromOutside.Events.Transmit
{
    /// <summary>
    /// Class that will hold Transmit Event
    /// </summary>
    public class TransmitEventHolder : IEventHolder
    {
        private readonly EventHandlerTransmit _eventHandlerTransmit = new();
        public ExternalEvents ExternalEvent => ExternalEvents.Transmit;
        public IExternalEventHandler ExternalEventHandler => _eventHandlerTransmit;
    }
}