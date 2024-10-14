using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Link
{
    public class LinkViewModel : ViewModelBase
    {
        public LinkViewModel(EventHandlerLink eventHandlerLink)
        {
            EventHandlerBase = eventHandlerLink;
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
            set => SetProperty(ref _isCurrentWorkset, value);
        }
    }
}