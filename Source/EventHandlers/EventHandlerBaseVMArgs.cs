using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VLS.BatchExportNet.Views;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public abstract class EventHandlerBaseVMArgs : RevitEventWrapper<ViewModelBase>
    {
        public abstract override void Execute(UIApplication app, ViewModelBase args);
    }
}
