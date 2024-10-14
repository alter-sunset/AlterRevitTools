using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Detach
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