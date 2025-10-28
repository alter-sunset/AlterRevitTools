using System.Windows;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Migrate;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandMigrateModels : ExternalCommandBase
{
    private protected override Func<Window> WindowFactory { get; } = () =>
        new MigrateModelsView(new EventHandlerMigrate());
}