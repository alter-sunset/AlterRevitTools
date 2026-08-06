using System.Reflection;
using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Interfaces;
using AlterTools.Utils.Extensions;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Utils;

public static class UtilsIFC
{
    public static IFCExportOptions GetIFCExportOptions(ConfigExportSingle config, Document doc)
    {
        IConfigIFC configIFC = config.ConfigIFC;
        IConfigIFCAdditionalFields configIFCAdd = config.ConfigIFCAdditionalFields;
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
}