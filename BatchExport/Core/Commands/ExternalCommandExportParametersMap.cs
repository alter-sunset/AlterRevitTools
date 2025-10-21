using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Params;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.ReadOnly)]
public class ExternalCommandExportParametersMap : IExternalCommand
{
    public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements)
    {
        return CommandWrapper.Execute(ref msg, () => new ExportParamsView(new EventHandlerParams()));
    }
}