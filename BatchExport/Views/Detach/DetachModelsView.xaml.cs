using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.Detach
{
    public partial class DetachModelsView
    {
        public DetachModelsView(EventHandlerDetach eventHandlerDetach)
        {
            InitializeComponent();
            DataContext = new DetachViewModel(eventHandlerDetach);
        }
    }
}