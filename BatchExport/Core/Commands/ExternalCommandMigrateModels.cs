using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Core.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ExternalCommandMigrateModels : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements)
            => CommandWrapper.Execute(ref msg, Forms.Migrate);
    }
}