using System.ComponentModel;
using System.Windows;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Transmit
{
    /// <summary>
    /// Interaction logic for DetachModelsView.xaml
    /// </summary>
    public partial class TransmitModelsView : Window
    {
        public TransmitModelsView(EventHandlerTransmitModelsVMArg eventHandlerTransmitModelsVMArg)
        {
            InitializeComponent();
            DataContext = new TransmitViewModel(eventHandlerTransmitModelsVMArg);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            DataContext = null;
            base.OnClosing(e);
        }
    }
}