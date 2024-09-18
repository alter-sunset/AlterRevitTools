using System.Windows;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Link
{
    /// <summary>
    /// Interaction logic for DetachModelsUi.xaml
    /// </summary>
    public partial class LinkModelsUi : Window
    {
        private readonly EventHandlerLinkModelsVMArg _eventHandlerLinkModelsUiArg;
        public LinkModelsUi(EventHandlerLinkModelsVMArg eventHandlerLinkModelsUiArg)
        {
            InitializeComponent();
            _eventHandlerLinkModelsUiArg = eventHandlerLinkModelsUiArg;
            DataContext = new LinkViewModel(_eventHandlerLinkModelsUiArg);
        }
    }
}