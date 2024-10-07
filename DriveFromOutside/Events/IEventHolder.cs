using Autodesk.Revit.UI;

namespace VLS.DriveFromOutside.Events
{
    public interface IEventHolder
    {
        public ExternalEvents ExternalEvent { get; }
        public IExternalEventHandler ExternalEventHandler { get; }
    }
}