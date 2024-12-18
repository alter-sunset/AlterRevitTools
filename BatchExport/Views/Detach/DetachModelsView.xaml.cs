using AlterTools.BatchExport.Source.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Detach
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