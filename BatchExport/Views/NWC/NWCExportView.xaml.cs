using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.NWC
{
    public partial class NwcExportView
    {
        public NwcExportView(EventHandlerNwc eventHandlerNwc, EventHandlerNwcBatch eventHandlerNwcBatch)
        {
            InitializeComponent();
            DataContext = new NwcViewModel(eventHandlerNwcBatch, eventHandlerNwc);
        }
    }
}