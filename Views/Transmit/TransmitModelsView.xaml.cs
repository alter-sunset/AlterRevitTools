using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Transmit
{
    /// <summary>
    /// Interaction logic for DetachModelsView.xaml
    /// </summary>
    public partial class TransmitModelsView : WindowBase
    {
        public TransmitModelsView(EventHandlerTransmitModelsVMArg eventHandlerTransmitModelsVMArg)
        {
            InitializeComponent();
            DataContext = new TransmitViewModel(eventHandlerTransmitModelsVMArg);
        }
    }
}