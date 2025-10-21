using System.Windows;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.IFC;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandExportModelsToIFC : ExternalCommandBase
{
    private protected override Func<Window> WindowFactory { get; } = () =>
        new IFCExportView(new EventHandlerIFC());
}