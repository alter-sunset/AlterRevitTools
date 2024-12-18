using Autodesk.Revit.UI;
using AlterTools.BatchExportNet.Views.Base;

namespace AlterTools.BatchExportNet.Source.EventHandlers
{
    public abstract class EventHandlerBase : RevitEventWrapper<IConfigBase>
    {
        public abstract override void Execute(UIApplication app, IConfigBase args);
    }
}