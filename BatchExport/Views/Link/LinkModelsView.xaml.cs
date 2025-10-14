using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.Link;

public partial class LinkModelsView
{
    public LinkModelsView(EventHandlerLink eventHandlerLink, Workset[] worksets)
    {
        InitializeComponent();
        DataContext = new LinkViewModel(eventHandlerLink, worksets);
    }
}