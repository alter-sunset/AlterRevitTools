using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.NWC
{
    public partial class NWCExportView
    {
        public NWCExportView(EventHandlerNWC eventHandlerNwc, EventHandlerNWC_Batch eventHandlerNwcBatch)
        {
            InitializeComponent();
            DataContext = new NWC_ViewModel(eventHandlerNwcBatch, eventHandlerNwc);
        }
    }
}