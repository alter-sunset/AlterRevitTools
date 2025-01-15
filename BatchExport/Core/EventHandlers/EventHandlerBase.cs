using Autodesk.Revit.UI;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Core.EventHandlers
{
    public abstract class EventHandlerBase : RevitEventWrapper<IConfigBase>
    {
        public abstract override void Execute(UIApplication app, IConfigBase args);
    }
}