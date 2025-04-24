using AlterTools.BatchExport.Core.EventHandlers;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.Link;

public partial class LinkModelsView
{
    public LinkModelsView(EventHandlerLink eventHandlerLink, Workset[] worksets)
    {
        InitializeComponent();
        DataContext = new LinkViewModel(eventHandlerLink, worksets);
    }
}