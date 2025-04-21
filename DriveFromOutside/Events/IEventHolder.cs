using Autodesk.Revit.UI;

namespace AlterTools.DriveFromOutside.Events;

public interface IEventHolder
{
    public ExternalEvents ExternalEvent { get; }
    public IExternalEventHandler ExternalEventHandler { get; }
}