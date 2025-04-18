using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.Transmit
{
    public partial class TransmitModelsView
    {
        public TransmitModelsView(EventHandlerTransmit eventHandlerTransmit)
        {
            InitializeComponent();
            DataContext = new TransmitViewModel(eventHandlerTransmit);
        }
    }
}