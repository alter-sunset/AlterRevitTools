using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Transmit
{
    public class TransmitViewModel : ViewModelBase
    {
        public TransmitViewModel(EventHandlerTransmit eventHandlerTransmit)
        {
            EventHandlerBase = eventHandlerTransmit;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessageType.TransmitTitle,
                    HelpMessageType.Load,
                    HelpMessageType.Folder,
                    HelpMessageType.List,
                    HelpMessageType.Start);
        }

        private bool _isSameFolder;
        public bool IsSameFolder
        {
            get => _isSameFolder;
            set => SetProperty(ref _isSameFolder, value);
        }
    }
}