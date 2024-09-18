using System.Windows;
using VLS.BatchExportNet.Source;

namespace VLS.BatchExportNet.Views.Link
{
    /// <summary>
    /// Interaction logic for DetachModelsUi.xaml
    /// </summary>
    public partial class LinkModelsUi : Window
    {
        private readonly EventHandlerLinkModelsUiArg _eventHandlerLinkModelsUiArg;
        public LinkModelsUi(EventHandlerLinkModelsUiArg eventHandlerLinkModelsUiArg)
        {
            InitializeComponent();
            _eventHandlerLinkModelsUiArg = eventHandlerLinkModelsUiArg;
            DataContext = new LinkViewModel(_eventHandlerLinkModelsUiArg);
        }
    }
}