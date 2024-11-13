using Autodesk.Revit.DB;
using System.Linq;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.IFC
{
    public class IFCHelper : ExportHelperBase
    {
        public override void ExportModel(IConfigBase_Extended iConfig, Document doc, ref bool isFuckedUp, ref Logger log)
        {
            IConfigIFC configIFC = iConfig as IConfigIFC;
            if (IsViewEmpty(iConfig, doc, ref log, ref isFuckedUp)) return;

            IFCExportOptions ifcExportOptions = IFC_ExportOptions(configIFC, doc);

            using Transaction t = new(doc);
            t.Start("Экспорт IFC");
            Export(iConfig, doc, ifcExportOptions, ref log, ref isFuckedUp);
            t.Commit();
        }
        private static IFCExportOptions IFC_ExportOptions(IConfigIFC configIFC, Document doc) => new()
        {
            ExportBaseQuantities = configIFC.ExportBaseQuantities,
            FamilyMappingFile = configIFC.FamilyMappingFile,
            FileVersion = configIFC.FileVersion,
            FilterViewId = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .FirstOrDefault(e => e.Name == configIFC.ViewName)
                .Id,
            SpaceBoundaryLevel = configIFC.SpaceBoundaryLevel,
            WallAndColumnSplitting = configIFC.WallAndColumnSplitting
        };
    }
}