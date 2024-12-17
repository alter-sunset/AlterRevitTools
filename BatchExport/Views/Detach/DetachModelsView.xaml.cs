using VLS.BatchExport.Source.EventHandlers;
using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.Detach
{
    /// <summary>
    /// Interaction logic for DetachModelsView.xaml
    /// </summary>
    public partial class DetachModelsView : WindowBase
    {
        public DetachModelsView(EventHandlerDetach eventHandlerDetach)
        {
            InitializeComponent();
            DataContext = new DetachViewModel(eventHandlerDetach);
        }
    }
}