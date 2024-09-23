using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.NWC
{
    /// <summary>
    /// Interaction logic for NWCExportView.xaml
    /// </summary>
    public partial class NWCExportView : WindowBase
    {
        public NWCExportView(EventHandlerNWCExportVMArg eventHandlerNWCExportVMArg, EventHandlerNWCExportBatchVMArg eventHandlerNWCExportBatchVMArg)
        {
            InitializeComponent();
            DataContext = new NWC_ViewModel(eventHandlerNWCExportBatchVMArg, eventHandlerNWCExportVMArg);
        }
    }
}