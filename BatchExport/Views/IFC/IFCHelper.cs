using Autodesk.Revit.DB;
using System.Linq;
using VLS.BatchExport.Utils;
using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.IFC
{
    public class IFCHelper : ExportHelperBase
    {
        public override void ExportModel(IConfigBase_Extended iConfig, Document doc, ref bool isFuckedUp, ref Logger log)
        {
            if (!(iConfig is IConfigIFC configIfc)
                || IsViewEmpty(iConfig, doc, ref log, ref isFuckedUp)) return;

            IFCExportOptions options = IFC_ExportOptions(configIfc, doc);

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Экспорт IFC");
                Export(iConfig, doc, options, ref log, ref isFuckedUp);
                t.Commit();
            }
        }
        private static IFCExportOptions IFC_ExportOptions(IConfigIFC config, Document doc) => new IFCExportOptions()
        {
            ExportBaseQuantities = config.ExportBaseQuantities,
            FamilyMappingFile = config.FamilyMappingFile,
            FileVersion = config.FileVersion,
            FilterViewId = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .FirstOrDefault(e => e.Name == config.ViewName)
                .Id,
            SpaceBoundaryLevel = config.SpaceBoundaryLevel,
            WallAndColumnSplitting = config.WallAndColumnSplitting
        };
    }
}