using VLS.BatchExport.Source.EventHandlers;
using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.Transmit
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