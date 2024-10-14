using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.NWC
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