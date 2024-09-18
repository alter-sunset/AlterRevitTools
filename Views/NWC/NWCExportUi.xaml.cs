using System.Windows;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.NWC
{
    /// <summary>
    /// Interaction logic for NWCExportUi.xaml
    /// </summary>
    public partial class NWCExportUi : Window
    {
        public NWCExportUi(EventHandlerNWCExportVMArg eventHandlerNWCExportUiArg, EventHandlerNWCExportBatchVMArg eventHandlerNWCExportBatchUiArg)
        {
            InitializeComponent();
            DataContext = new NWC_ViewModel(eventHandlerNWCExportBatchUiArg, eventHandlerNWCExportUiArg);
        }
    }
}