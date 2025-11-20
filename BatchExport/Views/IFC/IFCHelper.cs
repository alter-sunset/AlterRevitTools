using AlterTools.BatchExport.Utils.Extensions;
using AlterTools.BatchExport.Utils.Logger;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.IFC;

public class IFCHelper : ExportHelperBase
{
    private protected override void ExportModel(IConfigBaseExtended iConfig, Document doc, ref bool isFuckedUp, ref ILogger log)
    {
        if (iConfig is null || doc is null) return;

        if (iConfig is not IConfigIFC configIFC || !IsViewReadyForExport(iConfig, doc, ref log, ref isFuckedUp)) return;

        using IFCExportOptions options = GetIFCExportOptions(configIFC, doc);

        using Transaction tr = new(doc);
        tr.Start(Resources.Strings.IFCTitle);

        Export(iConfig, doc, options, ref log, ref isFuckedUp);

        tr.Commit();
    }

    private static IFCExportOptions GetIFCExportOptions(IConfigIFC config, Document doc)
    {
        IFCExportOptions options = new()
        {
            ExportBaseQuantities = config.ExportBaseQuantities,
            FamilyMappingFile = config.FamilyMappingFile,
            FileVersion = config.FileVersion,
            SpaceBoundaryLevel = config.SpaceBoundaryLevel,
            WallAndColumnSplitting = config.WallAndColumnSplitting
        };

        if (config.ExportScopeView && doc.DoesViewExist(config.ViewName))
        {
            options.FilterViewId = new FilteredElementCollector(doc)
                .OfClass(typeof(View3D))
                .FirstOrDefault(el => el.Name == config.ViewName && !((View3D)el).IsTemplate)
                .Id;
        }
        
        return options;
    }
}