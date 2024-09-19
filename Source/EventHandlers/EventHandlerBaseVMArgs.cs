using Autodesk.Revit.UI;
using VLS.BatchExportNet.Views;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public abstract class EventHandlerBaseVMArgs : RevitEventWrapper<ViewModelBase>
    {
        public abstract override void Execute(UIApplication app, ViewModelBase args);
    }
}