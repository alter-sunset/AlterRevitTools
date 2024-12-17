using VLS.BatchExport.Source.EventHandlers;
using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.IFC
{
    /// <summary>
    /// Interaction logic for NWCExportView.xaml
    /// </summary>
    public partial class IFCExportView : WindowBase
    {
        public IFCExportView(EventHandlerIFC eventHandlerIFC)
        {
            InitializeComponent();
            DataContext = new IFC_ViewModel(eventHandlerIFC);
        }
    }
}