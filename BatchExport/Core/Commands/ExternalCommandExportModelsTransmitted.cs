using System.Windows;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Transmit;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandExportModelsTransmitted : ExternalCommandBase
{
    protected override Func<Window> WindowFactory { get; } = () => new TransmitModelsView(new EventHandlerTransmit());
}