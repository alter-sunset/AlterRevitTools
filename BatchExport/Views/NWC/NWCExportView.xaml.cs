using VLS.BatchExport.Source.EventHandlers;
using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.NWC
{
    /// <summary>
    /// Interaction logic for NWCExportView.xaml
    /// </summary>
    public partial class NWCExportView : WindowBase
    {
        public NWCExportView(EventHandlerNWC eventHandlerNWC, EventHandlerNWC_Batch eventHandlerNWC_Batch)
        {
            InitializeComponent();
            DataContext = new NWC_ViewModel(eventHandlerNWC_Batch, eventHandlerNWC);
        }
    }
}