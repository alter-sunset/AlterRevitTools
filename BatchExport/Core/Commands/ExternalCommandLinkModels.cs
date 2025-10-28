using System.Windows;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Link;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandLinkModels : ExternalCommandBase
{
    private protected override Func<Window> WindowFactory { get; } = () =>
        new LinkModelsView(new EventHandlerLink(), GetWorksets(CommandData.Application));
    
    private static Workset[] GetWorksets(UIApplication uiApp)
    {
        return [.. new FilteredWorksetCollector(uiApp.ActiveUIDocument.Document)
            .OfKind(WorksetKind.UserWorkset)
            .ToWorksets()];
    }
}