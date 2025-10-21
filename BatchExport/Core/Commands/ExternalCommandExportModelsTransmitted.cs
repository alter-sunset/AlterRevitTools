using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Transmit;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandExportModelsTransmitted : IExternalCommand
{
    public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements)
    {
        return CommandWrapper.Execute(ref msg, () => new TransmitModelsView(new EventHandlerTransmit()));
    }
}