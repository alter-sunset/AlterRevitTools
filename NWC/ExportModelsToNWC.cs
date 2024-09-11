using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.NWC
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ExportModelsToNWC : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                App.ShowFormNWC(commandData.Application);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }

    public class ExportModelsToNWCCommand_Availability : CommandAvailabilityWrapper { }
}