using Autodesk.Revit.DB;
using AlterTools.BatchExportNet.Source.EventHandlers;
using AlterTools.BatchExportNet.Views.Base;

namespace AlterTools.BatchExportNet.Views.Link
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