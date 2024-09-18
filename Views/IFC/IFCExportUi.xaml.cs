using System.Windows;
using VLS.BatchExportNet.Source;

namespace VLS.BatchExportNet.Views.IFC
{
    /// <summary>
    /// Interaction logic for NWCExportUi.xaml
    /// </summary>
    public partial class IFCExportUi : Window
    {
        public IFCExportUi(EventHandlerIFCExportUiArg eventHandlerIFCExportUiArg)
        {
            InitializeComponent();
            DataContext = new IFC_ViewModel(eventHandlerIFCExportUiArg);
        }
    }
}