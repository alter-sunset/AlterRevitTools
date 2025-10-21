using System.Windows;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Params;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.ReadOnly)]
public class ExternalCommandExportParametersMap : ExternalCommandBase
{
    internal override Func<Window> WindowFactory { get; } = () => new ExportParamsView(new EventHandlerParams());
}