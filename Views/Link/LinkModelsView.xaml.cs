using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Link
{
    /// <summary>
    /// Interaction logic for DetachModelsView.xaml
    /// </summary>
    public partial class LinkModelsView : WindowBase
    {
        public LinkModelsView(EventHandlerLinkModelsVMArg eventHandlerLinkModelsVMArg)
        {
            InitializeComponent();
            DataContext = new LinkViewModel(eventHandlerLinkModelsVMArg);
        }
    }
}