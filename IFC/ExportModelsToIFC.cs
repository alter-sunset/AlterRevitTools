using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.IFC
{
    [Transaction(TransactionMode.Manual)]
    public class ExportModelsToIFC : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                App.ShowForm(commandData.Application, Forms.IFC);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}