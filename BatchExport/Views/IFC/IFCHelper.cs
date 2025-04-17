using Autodesk.Revit.DB;
using System.Linq;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.IFC
{
    public class IfcHelper : ExportHelperBase
    {
        protected override void ExportModel(IConfigBaseExtended iConfig, Document doc, ref bool isFuckedUp, ref Logger log)
        {
            if ((iConfig is not IConfigIfc configIfc)
                || IsViewEmpty(iConfig, doc, ref log, ref isFuckedUp)) return;

            IFCExportOptions options = IFC_ExportOptions(configIfc, doc);

            using Transaction tr = new(doc);
            tr.Start("Экспорт IFC");

            Export(iConfig, doc, options, ref log, ref isFuckedUp);

            tr.Commit();
        }
        private static IFCExportOptions IFC_ExportOptions(IConfigIfc config, Document doc) => new()
        {
            ExportBaseQuantities = config.ExportBaseQuantities,
            FamilyMappingFile = config.FamilyMappingFile,
            FileVersion = config.FileVersion,

            FilterViewId = new FilteredElementCollector(doc)
                               .OfClass(typeof(View))
                               .FirstOrDefault(el => el.Name == config.ViewName)
                               .Id,

            SpaceBoundaryLevel = config.SpaceBoundaryLevel,
            WallAndColumnSplitting = config.WallAndColumnSplitting
        };
    }
}