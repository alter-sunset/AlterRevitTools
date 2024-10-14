using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.IFC
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