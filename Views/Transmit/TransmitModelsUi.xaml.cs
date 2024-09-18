using System.Windows;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Transmit
{
    /// <summary>
    /// Interaction logic for DetachModelsUi.xaml
    /// </summary>
    public partial class TransmitModelsUi : Window
    {
        public TransmitModelsUi(EventHandlerTransmitModelsVMArg eventHandlerTransmitModelsUiArg)
        {
            InitializeComponent();
            DataContext = new TransmitViewModel(eventHandlerTransmitModelsUiArg);
        }
    }
}