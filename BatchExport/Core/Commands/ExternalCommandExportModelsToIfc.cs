using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JetBrains.Annotations;

namespace AlterTools.BatchExport.Core.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class ExternalCommandExportModelsToIfc : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements)
        {
            return CommandWrapper.Execute(ref msg, Forms.Ifc);
        }
    }
}