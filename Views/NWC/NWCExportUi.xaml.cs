using System.Windows;
using VLS.BatchExportNet.Source;

namespace VLS.BatchExportNet.Views.NWC
{
    /// <summary>
    /// Interaction logic for NWCExportUi.xaml
    /// </summary>
    public partial class NWCExportUi : Window
    {
        public NWCExportUi(EventHandlerNWCExportUiArg eventHandlerNWCExportUiArg, EventHandlerNWCExportBatchUiArg eventHandlerNWCExportBatchUiArg)
        {
            InitializeComponent();
            DataContext = new NWC_ViewModel(eventHandlerNWCExportBatchUiArg, eventHandlerNWCExportUiArg);
        }
    }
}