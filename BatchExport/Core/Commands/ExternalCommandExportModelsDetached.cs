using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Core.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ExternalCommandExportModelsDetached : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements)
        {
            return CommandWrapper.Execute(ref msg, Forms.Detach);
        }
    }
}