using System.Collections.Generic;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Transmit
{
    public class TransmitViewModel : ViewModelBase
    {
        public TransmitViewModel(EventHandlerTransmitModelsVMArg eventHandlerTransmitModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerTransmitModelsUiArg;
            Dictionary<HelpMessages, string> help = Help.GetHelpMessages();
            string _helpMessage = help.GetValueOrDefault(HelpMessages.TransmitTitle) +
                "\n" + help.GetValueOrDefault(HelpMessages.Load) +
                "\n" + help.GetValueOrDefault(HelpMessages.Folder) +
                "\n" + help.GetValueOrDefault(HelpMessages.List) +
                "\n" + help.GetValueOrDefault(HelpMessages.Start);
            HelpMessage = _helpMessage;
        }

        private bool _isSameFolder;
        public bool IsSameFolder
        {
            get => _isSameFolder;
            set
            {
                _isSameFolder = value;
                OnPropertyChanged("IsSameFolder");
            }
        }
    }
}