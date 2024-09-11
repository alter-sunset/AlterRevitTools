using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Detach
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ExportModelsDetached : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                App.ShowFormDetach(commandData.Application);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
    public class ExportModelsDetachedCommand_Availability : CommandAvailabilityWrapper { }
}