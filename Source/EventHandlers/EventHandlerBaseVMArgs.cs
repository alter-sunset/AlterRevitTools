using Autodesk.Revit.UI;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public abstract class EventHandlerBaseVMArgs : RevitEventWrapper<ViewModelBase>
    {
        public abstract override void Execute(UIApplication app, ViewModelBase args);
    }
}