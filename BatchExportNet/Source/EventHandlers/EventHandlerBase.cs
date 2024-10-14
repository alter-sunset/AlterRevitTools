using Autodesk.Revit.UI;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public abstract class EventHandlerBase : RevitEventWrapper<IConfigBase>
    {
        public abstract override void Execute(UIApplication app, IConfigBase args);
    }
}