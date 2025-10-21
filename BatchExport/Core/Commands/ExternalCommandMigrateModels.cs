using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Migrate;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandMigrateModels : IExternalCommand
{
    public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements)
    {
        return CommandWrapper.Execute(ref msg, () => new MigrateModelsView(new EventHandlerMigrate()));
    }
}