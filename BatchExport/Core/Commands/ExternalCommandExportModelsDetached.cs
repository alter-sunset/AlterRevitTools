using System.Windows;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Detach;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandExportModelsDetached : ExternalCommandBase
{
    private protected override Func<Window> WindowFactory { get; } = () =>
        new DetachModelsView(new EventHandlerDetach());
}