using AlterTools.atExportModel.Configs;
using AlterTools.Utils.Extensions;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Utils;

public static class UtilsNWC
{
    public static NavisworksExportOptions GetNWCExportOptions(ConfigExportSingle config, Document doc)
    {
        ConfigNWC configNWC = config.ConfigNWC;
        NavisworksExportOptions options = new()
        {
            ConvertElementProperties = configNWC.ConvertElementProperties,
            DivideFileIntoLevels = configNWC.DivideFileIntoLevels,
            ExportElementIds = configNWC.ExportElementIds,
            ExportLinks = configNWC.ExportLinks,
            ExportParts = configNWC.ExportParts,
            ExportRoomAsAttribute = configNWC.ExportRoomAsAttribute,
            ExportRoomGeometry = configNWC.ExportRoomGeometry,
            ExportUrls = configNWC.ExportUrls,
            FindMissingMaterials = configNWC.FindMissingMaterials,
            Coordinates = configNWC.Coordinates,
            Parameters = configNWC.Parameters,
            ExportScope = configNWC.ExportScope,

#if R20_OR_GREATER
            ConvertLights = configNWC.ConvertLights,
            ConvertLinkedCADFormats = configNWC.ConvertLinkedCADFormats,
            FacetingFactor = configNWC.FacetingFactor,
#endif
        };

        if (configNWC.ExportScope == NavisworksExportScope.View && doc.DoesViewExist(config.ViewName))
        {
            options.ExportScope = NavisworksExportScope.View;
            options.ViewId = new FilteredElementCollector(doc)
                .OfClass(typeof(View3D))
                .FirstOrDefault(el => el.Name == config.ViewName && !((View3D)el).IsTemplate)
                .Id;
        }
        else
        {
            options.ExportScope = NavisworksExportScope.Model;
        }

        return options;
    }
}