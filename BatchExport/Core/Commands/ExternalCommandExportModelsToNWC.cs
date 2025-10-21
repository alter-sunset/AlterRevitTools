using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.NWC;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandExportModelsToNWC : IExternalCommand
{
    public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements)
    {
        return CommandWrapper.Execute(ref msg,
            () => new NWCExportView(new EventHandlerNWC(), new EventHandlerNWCBatch()));
    }
}