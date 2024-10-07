using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Link
{
    public class LinkViewModel : ViewModelBase
    {
        public LinkViewModel(EventHandlerLinkModelsVMArg eventHandlerLinkModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerLinkModelsUiArg;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessageType.LinkTitle,
                    HelpMessageType.Load,
                    HelpMessageType.List,
                    HelpMessageType.Start);
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