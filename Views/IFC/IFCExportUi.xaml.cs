using System.Windows;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.IFC
{
    /// <summary>
    /// Interaction logic for NWCExportUi.xaml
    /// </summary>
    public partial class IFCExportUi : Window
    {
        public IFCExportUi(EventHandlerIFCExportVMArg eventHandlerIFCExportUiArg)
        {
            InitializeComponent();
            DataContext = new IFC_ViewModel(eventHandlerIFCExportUiArg);
        }
    }
}