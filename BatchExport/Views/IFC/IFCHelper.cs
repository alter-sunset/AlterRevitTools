using System.Linq;
using AlterTools.BatchExport.Utils.Logger;
using AlterTools.BatchExport.Views.Base;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.IFC;

public class IFCHelper : ExportHelperBase
{
    protected override void ExportModel(IConfigBaseExtended iConfig, Document doc, ref bool isFuckedUp,
        ref ILogger log)
    {
        if (iConfig is null || doc is null) return;

        if (iConfig is not IConfigIFC configIFC
            || IsViewEmpty(iConfig, doc, ref log, ref isFuckedUp))
            return;

        IFCExportOptions options = IFC_ExportOptions(configIFC, doc);

        using Transaction tr = new(doc);
        tr.Start("Экспорт IFC");

        Export(iConfig, doc, options, ref log, ref isFuckedUp);

        tr.Commit();
    }

    private static IFCExportOptions IFC_ExportOptions(IConfigIFC config, Document doc)
    {
        return new IFCExportOptions
        {
            ExportBaseQuantities = config.ExportBaseQuantities,
            FamilyMappingFile = config.FamilyMappingFile,
            FileVersion = config.FileVersion,
            FilterViewId = GetViewId(config.ViewName, doc),
            SpaceBoundaryLevel = config.SpaceBoundaryLevel,
            WallAndColumnSplitting = config.WallAndColumnSplitting
        };
    }

    private static ElementId GetViewId(string viewName, Document doc)
    {
        if (doc is null || string.IsNullOrEmpty(viewName)) return null;

        return new FilteredElementCollector(doc)
            .OfClass(typeof(View))
            .FirstOrDefault(el => el.Name == viewName)?
            .Id;
    }
}