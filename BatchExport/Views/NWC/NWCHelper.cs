using AlterTools.BatchExport.Utils.Extensions;
using AlterTools.BatchExport.Utils.Logger;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.NWC;

public class NWCHelper : ExportHelperBase
{
    private protected override void ExportModel(IConfigBaseExtended iConfig, Document doc, ref bool isFuckedUp, ref ILogger log)
    {
        if (iConfig is not IConfigNWC configNWC) return;

        if (!IsViewReadyForExport(iConfig, doc, ref log, ref isFuckedUp)) return;

        NavisworksExportOptions options = NWC_ExportOptions(configNWC, doc);

        Export(iConfig, doc, options, ref log, ref isFuckedUp);
    }

    private static NavisworksExportOptions NWC_ExportOptions(IConfigNWC config, Document doc)
    {
        NavisworksExportOptions options = new()
        {
            ConvertElementProperties = config.ConvertElementProperties,
            DivideFileIntoLevels = config.DivideFileIntoLevels,
            ExportElementIds = config.ExportElementIds,
            ExportLinks = config.ExportLinks,
            ExportParts = config.ExportParts,
            ExportRoomAsAttribute = config.ExportRoomAsAttribute,
            ExportRoomGeometry = config.ExportRoomGeometry,
            ExportUrls = config.ExportUrls,
            FindMissingMaterials = config.FindMissingMaterials,
            Coordinates = config.Coordinates,
            Parameters = config.Parameters,
            ExportScope = config.ExportScopeView
                ? NavisworksExportScope.View
                : NavisworksExportScope.Model,

#if R20_OR_GREATER
            ConvertLights = config.ConvertLights,
            ConvertLinkedCADFormats = config.ConvertLinkedCADFormats,
            FacetingFactor = config.FacetingFactor,
#endif
        };

        if (config.ExportScopeView && doc.DoesViewExist(config.ViewName))
        {
            options.ExportScope = NavisworksExportScope.View;
            options.ViewId = new FilteredElementCollector(doc)
                .OfClass(typeof(View3D))
                .FirstOrDefault(el => el.Name == config.ViewName && !((View3D)el).IsTemplate)
                .Id;
        }
        else if (config.IgnoreMissingView)
        {
            options.ExportScope =  NavisworksExportScope.Model;
        }

        return options;
    }
}