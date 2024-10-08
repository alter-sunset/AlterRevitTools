﻿using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Transmit
{
    public class TransmitViewModel : ViewModelBase
    {
        public TransmitViewModel(EventHandlerTransmitModelsVMArg eventHandlerTransmitModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerTransmitModelsUiArg;
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
            set
            {
                _isSameFolder = value;
                OnPropertyChanged(nameof(IsSameFolder));
            }
        }
    }
}