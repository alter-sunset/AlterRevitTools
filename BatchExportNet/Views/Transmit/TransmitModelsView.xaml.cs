using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Transmit
{
    /// <summary>
    /// Interaction logic for DetachModelsView.xaml
    /// </summary>
    public partial class TransmitModelsView : WindowBase
    {
        public TransmitModelsView(EventHandlerTransmit eventHandlerTransmit)
        {
            InitializeComponent();
            DataContext = new TransmitViewModel(eventHandlerTransmit);
        }
    }
}