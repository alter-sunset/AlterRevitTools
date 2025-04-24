using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.NWC;

public partial class NWCExportView
{
    public NWCExportView(EventHandlerNWC eventHandlerNWC, EventHandlerNWCBatch eventHandlerNWCBatch)
    {
        InitializeComponent();
        DataContext = new NWCViewModel(eventHandlerNWCBatch, eventHandlerNWC);
    }
}