using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Detach
{
    public partial class DetachModelsView : WindowBase
    {
        public DetachModelsView(EventHandlerDetach eventHandlerDetach)
        {
            InitializeComponent();
            DataContext = new DetachViewModel(eventHandlerDetach);
        }
    }
}