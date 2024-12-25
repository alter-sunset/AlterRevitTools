using System.Linq;
using Autodesk.Revit.DB;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.NWC
{
    public class NWCHelper : ExportHelperBase
    {
        public override void ExportModel(IConfigBase_Extended iConfig, Document doc, ref bool isFuckedUp, ref Logger log)
        {
            if (iConfig is not IConfigNWC configNwc
                || IsViewEmpty(iConfig, doc, ref log, ref isFuckedUp)) return;

            NavisworksExportOptions options = NWC_ExportOptions(configNwc, doc);

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

#if R20_OR_GREATER
                ConvertLights = config.ConvertLights,
                ConvertLinkedCADFormats = config.ConvertLinkedCADFormats,
                FacetingFactor = config.FacetingFactor,
#endif

                Coordinates = config.Coordinates,
                Parameters = config.Parameters,
                ExportScope = config.ExportScopeView
                    ? NavisworksExportScope.View
                    : NavisworksExportScope.Model

            };
            if (config.ExportScopeView)
                options.ViewId = new FilteredElementCollector(doc)
                        .OfClass(typeof(View3D))
                        .FirstOrDefault(e => e.Name == config.ViewName
                            && !((View3D)e).IsTemplate)
                        .Id;
            return options;
        }
    }
}