using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.NWC
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