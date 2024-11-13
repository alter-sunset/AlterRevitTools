using System.Linq;
using Autodesk.Revit.DB;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.NWC
{
    public class NWCHelper : ExportHelperBase
    {
        public override void ExportModel(IConfigBase_Extended iConfig, Document doc, ref bool isFuckedUp, ref Logger log)
        {
            IConfigNWC configNWC = iConfig as IConfigNWC;
            if (IsViewEmpty(iConfig, doc, ref log, ref isFuckedUp)) return;

            NavisworksExportOptions navisExportOptions = NWC_ExportOptions(configNWC, doc);

            Export(iConfig, doc, navisExportOptions, ref log, ref isFuckedUp);
        }
        private static NavisworksExportOptions NWC_ExportOptions(IConfigNWC configNWC, Document doc)
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
                options.ViewId = new FilteredElementCollector(doc)
                        .OfClass(typeof(View3D))
                        .FirstOrDefault(e => e.Name == configNWC.ViewName
                            && !((View3D)e).IsTemplate)
                        .Id;
            return options;
        }
    }
}