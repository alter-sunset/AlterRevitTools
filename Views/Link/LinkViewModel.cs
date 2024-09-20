﻿using System.Collections.Generic;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Link
{
    public class LinkViewModel : ViewModelBase
    {
        public LinkViewModel(EventHandlerLinkModelsVMArg eventHandlerLinkModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerLinkModelsUiArg;
            Dictionary<HelpMessages, string> help = Help.GetHelpMessages();
            string _helpMessage =
                help.GetResultMessage(HelpMessages.LinkTitle,
                    HelpMessages.Load,
                    HelpMessages.List,
                    HelpMessages.Start);
            HelpMessage = _helpMessage;
        }

        private bool _isCurrentWorkset = true;
        public bool IsCurrentWorkset
        {
            get => _isCurrentWorkset;
            set
            {
                _isCurrentWorkset = value;
                OnPropertyChanged("IsCurrentWorkset");
            }
        }
    }
}