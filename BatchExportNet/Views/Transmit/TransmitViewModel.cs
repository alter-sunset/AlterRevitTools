using AlterTools.BatchExportNet.Source.EventHandlers;
using AlterTools.BatchExportNet.Views.Base;

namespace AlterTools.BatchExportNet.Views.Transmit
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