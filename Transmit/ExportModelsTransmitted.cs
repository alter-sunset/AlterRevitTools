using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Transmit
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ExportModelsTransmitted : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                App.ShowFormTransmit(commandData.Application);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
    public class ExportModelsTransmittedCommand_Availability : CommandAvailabilityWrapper { }
}