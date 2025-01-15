using Autodesk.Revit.DB;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Link
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