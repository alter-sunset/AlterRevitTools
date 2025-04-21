using Autodesk.Revit.UI;

namespace AlterTools.DriveFromOutside.Events.Detach;

public class DetachEventHolder : IEventHolder
{
    private readonly EventHandlerDetach _eventHandlerDetach = new();
    public ExternalEvents ExternalEvent => ExternalEvents.Detach;
    public IExternalEventHandler ExternalEventHandler => _eventHandlerDetach;
}