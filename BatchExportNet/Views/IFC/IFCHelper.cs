using System.Linq;
using Autodesk.Revit.DB;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.IFC
{
    class IFCHelper : ExportHelperBase
    {
        public override void ExportModel(IConfigBase_Extended iConfig, Document document, ref bool isFuckedUp, ref Logger logger)
        {
            IFC_ViewModel ifc_ViewModel = iConfig as IFC_ViewModel;
            if (IsViewEmpty(iConfig, document, ref logger, ref isFuckedUp))
                return;

            IFCExportOptions ifcExportOptions = IFC_ExportOptions(ifc_ViewModel, document);

            using Transaction transaction = new(document);
            transaction.Start("Экспорт IFC");
            Export(iConfig, document, ifcExportOptions, ref logger, ref isFuckedUp);
            transaction.Commit();
        }
        private static IFCExportOptions IFC_ExportOptions(IFC_ViewModel ifc_ViewModel, Document document) => new()
        {
            ExportBaseQuantities = ifc_ViewModel.ExportBaseQuantities,
            FamilyMappingFile = ifc_ViewModel.Mapping,
            FileVersion = ifc_ViewModel.SelectedVersion.Key,
            FilterViewId = new FilteredElementCollector(document)
                .OfClass(typeof(View))
                .FirstOrDefault(e => e.Name == ifc_ViewModel.ViewName)
                .Id,
            SpaceBoundaryLevel = ifc_ViewModel.SelectedLevel.Key,
            WallAndColumnSplitting = ifc_ViewModel.WallAndColumnSplitting
        };
    }
}