using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Detach
{
    /// <summary>
    /// Interaction logic for DetachModelsView.xaml
    /// </summary>
    public partial class DetachModelsView : WindowBase
    {
        public DetachModelsView(EventHandlerDetachModelsVMArg eventHandlerDetachModelsVMArg)
        {
            InitializeComponent();
            DataContext = new DetachViewModel(eventHandlerDetachModelsVMArg);
        }
    }
}