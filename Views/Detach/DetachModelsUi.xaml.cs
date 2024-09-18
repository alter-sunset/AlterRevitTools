using System.Windows;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Detach
{
    /// <summary>
    /// Interaction logic for DetachModelsUi.xaml
    /// </summary>
    public partial class DetachModelsUi : Window
    {
        public DetachModelsUi(EventHandlerDetachModelsVMArg eventHandlerDetachModelsUiArg)
        {
            InitializeComponent();
            DataContext = new DetachViewModel(eventHandlerDetachModelsUiArg);
        }
    }
}