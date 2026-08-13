using System.IO;
using System.Reflection;
using AlterTools.atExportModel.Configs;
using AlterTools.Utils.Extensions;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Utils;

public static class UtilsIFC
{
    public static IFCExportOptions GetIFCExportOptions(ConfigExportSingle config, Document doc)
    {
        ConfigIFC configIFC = config.ConfigIFC;
        ConfigIFCAdd configIFCAdd = config.ConfigIFCAdditionalFields;
        IFCExportOptions options = new()
        {
            ExportBaseQuantities = configIFC.ExportBaseQuantities,
            FamilyMappingFile = configIFC.FamilyMappingFile,
            FileVersion = configIFC.FileVersion,
            SpaceBoundaryLevel = (int)configIFC.SpaceBoundaryLevel,
            WallAndColumnSplitting = configIFC.WallAndColumnSplitting
        };

        Dictionary<string, string> additionals = configIFCAdd.GetType()
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(
                f => f.Name,
                f => f.GetValue(configIFCAdd)?.ToString() ?? string.Empty
            );

        foreach (KeyValuePair<string, string> field in additionals)
        {
            options.AddOption(field.Key, field.Value);
        }

        if (configIFCAdd.UseActiveViewGeometry &&
            !string.IsNullOrWhiteSpace(config.ViewName) &&
            doc.DoesViewExist(config.ViewName))
        {
            options.FilterViewId = new FilteredElementCollector(doc)
                .OfClass(typeof(View3D))
                .FirstOrDefault(el => el.Name == config.ViewName && !((View3D)el).IsTemplate)
                .Id;
        }

        return options;
    }

    // Do I need it, or exporter will handle it?
    private static void ValidateConditionals(ConfigIFC configIFC, ConfigIFCAdd configIFCAdd)
    {
        if (string.IsNullOrWhiteSpace(configIFC.FamilyMappingFile)
            || !File.Exists(configIFC.FamilyMappingFile))
        {
            configIFC.UseUserMapping = false;
        }

        if (string.IsNullOrWhiteSpace(configIFCAdd.ExportUserDefinedPsetsFileName)
            || !File.Exists(configIFCAdd.ExportUserDefinedPsetsFileName))
        {
            configIFCAdd.ExportUserDefinedPsets = false;
        }
    }
}