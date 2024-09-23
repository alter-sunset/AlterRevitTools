using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Link
{
    public class LinkViewModel : ViewModelBase
    {
        public LinkViewModel(EventHandlerLinkModelsVMArg eventHandlerLinkModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerLinkModelsUiArg;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessages.LinkTitle,
                    HelpMessages.Load,
                    HelpMessages.List,
                    HelpMessages.Start);
        }

        private bool _isCurrentWorkset = true;
        public bool IsCurrentWorkset
        {
            get => _isCurrentWorkset;
            set
            {
                _isCurrentWorkset = value;
                OnPropertyChanged(nameof(IsCurrentWorkset));
            }
        }
    }
}