using Autodesk.Revit.DB;
using VLS.BatchExport.Source.EventHandlers;
using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.Link
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