using Autodesk.Revit.DB;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Link
{
    /// <summary>
    /// Interaction logic for DetachModelsView.xaml
    /// </summary>
    public partial class LinkModelsView : WindowBase
    {
        public LinkModelsView(EventHandlerLink eventHandlerLink, Workset[] worksets)
        {
            InitializeComponent();
            DataContext = new LinkViewModel(eventHandlerLink, worksets);
        }
    }
}