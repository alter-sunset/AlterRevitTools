using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Link;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandLinkModels : IExternalCommand
{
    public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements)
    {
        return CommandWrapper.Execute(ref msg,
            () => new LinkModelsView(new EventHandlerLink(), GetWorksets(commandData.Application)));
    }
    
    private static Workset[] GetWorksets(UIApplication uiApp)
    {
        return [.. new FilteredWorksetCollector(uiApp.ActiveUIDocument.Document)
            .OfKind(WorksetKind.UserWorkset)
            .ToWorksets()];
    }
}