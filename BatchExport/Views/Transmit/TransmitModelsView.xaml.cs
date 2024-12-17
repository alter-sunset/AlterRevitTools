using VLS.BatchExport.Source.EventHandlers;
using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.Transmit
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