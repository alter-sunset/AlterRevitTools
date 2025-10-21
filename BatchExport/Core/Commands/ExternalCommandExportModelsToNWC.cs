using System.Windows;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.NWC;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.ReadOnly)]
public class ExternalCommandExportModelsToNWC : ExternalCommandBase
{
    private protected override Func<Window> WindowFactory { get; } = () =>
        new NWCExportView(new EventHandlerNWC(), new EventHandlerNWCBatch());
}