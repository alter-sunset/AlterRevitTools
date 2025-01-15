using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Transmit
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