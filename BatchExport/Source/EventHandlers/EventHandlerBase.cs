using Autodesk.Revit.UI;
using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Source.EventHandlers
{
    public abstract class EventHandlerBase : RevitEventWrapper<IConfigBase>
    {
        public abstract override void Execute(UIApplication app, IConfigBase args);
    }
}