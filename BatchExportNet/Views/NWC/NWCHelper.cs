using System.Linq;
using Autodesk.Revit.DB;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.NWC
{
    public class NWCHelper : ExportHelperBase
    {
        public override void ExportModel(IConfigBase_Extended iConfig, Document document, ref bool isFuckedUp, ref Logger logger)
        {
            IConfigNWC configNWC = iConfig as IConfigNWC;
            if (IsViewEmpty(iConfig, document, ref logger, ref isFuckedUp)) return;

            NavisworksExportOptions navisworksExportOptions = NWC_ExportOptions(configNWC, document);

            Export(iConfig, document, navisworksExportOptions, ref logger, ref isFuckedUp);
        }
        private static NavisworksExportOptions NWC_ExportOptions(IConfigNWC configNWC, Document document)
        {
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
                ConvertLights = configNWC.ConvertLights,
                ConvertLinkedCADFormats = configNWC.ConvertLinkedCADFormats,
                Coordinates = configNWC.Coordinates,
                Parameters = configNWC.Parameters,
                FacetingFactor = configNWC.FacetingFactor,
                ExportScope = configNWC.ExportScopeView
                    ? NavisworksExportScope.View
                    : NavisworksExportScope.Model

            };
            if (configNWC.ExportScopeView)
                options.ViewId = new FilteredElementCollector(document)
                        .OfClass(typeof(View3D))
                        .FirstOrDefault(e => e.Name == configNWC.ViewName
                            && !((View3D)e).IsTemplate)
                        .Id;
            return options;
        }
    }
}