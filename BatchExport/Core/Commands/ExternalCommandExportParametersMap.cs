using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Core.Commands
{
    [Transaction(TransactionMode.ReadOnly)]
    public class ExternalCommandExportParametersMap : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements)
            => CommandWrapper.Execute(ref msg, Forms.Params);
    }
}