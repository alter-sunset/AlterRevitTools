using VLS.BatchExportNet.Source;
using System.Windows;

namespace VLS.BatchExportNet.Views.Detach
{
    /// <summary>
    /// Interaction logic for DetachModelsUi.xaml
    /// </summary>
    public partial class DetachModelsUi : Window
    {
        public DetachModelsUi(EventHandlerDetachModelsUiArg eventHandlerDetachModelsUiArg)
        {
            InitializeComponent();
            DataContext = new DetachViewModel(eventHandlerDetachModelsUiArg);
        }
    }
}