using System.ComponentModel;
using System.Windows;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.IFC
{
    /// <summary>
    /// Interaction logic for NWCExportView.xaml
    /// </summary>
    public partial class IFCExportView : Window
    {
        public IFCExportView(EventHandlerIFCExportVMArg eventHandlerIFCExportVMArg)
        {
            InitializeComponent();
            DataContext = new IFC_ViewModel(eventHandlerIFCExportVMArg);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            DataContext = null;
            base.OnClosing(e);
        }
    }
}