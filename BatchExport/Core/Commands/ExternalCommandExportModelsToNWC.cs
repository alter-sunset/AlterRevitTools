using System.Windows;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.NWC;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandExportModelsToNWC : ExternalCommandBase
{
    protected override Func<Window> WindowFactory { get; } =
        () => new NWCExportView(new EventHandlerNWC(), new EventHandlerNWCBatch());
}