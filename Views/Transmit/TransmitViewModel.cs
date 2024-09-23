using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Transmit
{
    public class TransmitViewModel : ViewModelBase
    {
        public TransmitViewModel(EventHandlerTransmitModelsVMArg eventHandlerTransmitModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerTransmitModelsUiArg;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessages.TransmitTitle,
                    HelpMessages.Load,
                    HelpMessages.Folder,
                    HelpMessages.List,
                    HelpMessages.Start);
        }

        private bool _isSameFolder;
        public bool IsSameFolder
        {
            get => _isSameFolder;
            set
            {
                _isSameFolder = value;
                OnPropertyChanged(nameof(IsSameFolder));
            }
        }
    }
}