using System.Linq;
using Autodesk.Revit.DB;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.IFC
{
    public class IFCHelper : ExportHelperBase
    {
        public override void ExportModel(IConfigBase_Extended iConfig, Document document, ref bool isFuckedUp, ref Logger logger)
        {
            IConfigIFC configIFC = iConfig as IConfigIFC;
            if (IsViewEmpty(iConfig, document, ref logger, ref isFuckedUp)) return;

            IFCExportOptions ifcExportOptions = IFC_ExportOptions(configIFC, document);

            using Transaction transaction = new(document);
            transaction.Start("Экспорт IFC");
            Export(iConfig, document, ifcExportOptions, ref logger, ref isFuckedUp);
            transaction.Commit();
        }
        private static IFCExportOptions IFC_ExportOptions(IConfigIFC configIFC, Document document) => new()
        {
            ExportBaseQuantities = configIFC.ExportBaseQuantities,
            FamilyMappingFile = configIFC.FamilyMappingFile,
            FileVersion = configIFC.FileVersion,
            FilterViewId = new FilteredElementCollector(document)
                .OfClass(typeof(View))
                .FirstOrDefault(e => e.Name == configIFC.ViewName)
                .Id,
            SpaceBoundaryLevel = configIFC.SpaceBoundaryLevel,
            WallAndColumnSplitting = configIFC.WallAndColumnSplitting
        };
    }
}