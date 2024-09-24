using System.Linq;
using Autodesk.Revit.DB;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.NWC
{
    class NWCHelper : ExportHelperBase
    {
        public override void ExportModel(ViewModelBase_Extended viewModel, Document document, ref bool isFuckedUp, ref Logger logger)
        {
            NWC_ViewModel nwc_ViewModel = viewModel as NWC_ViewModel;
            Element view = GetView(nwc_ViewModel, document);

            if (nwc_ViewModel.ExportScopeView
                && !nwc_ViewModel.ExportLinks
                && document.IsViewEmpty(view))
            {
                logger.Error("Нет геометрии на виде.");
                isFuckedUp = true;
                return;
            }

            NavisworksExportOptions navisworksExportOptions = NWC_ExportOptions(nwc_ViewModel, document);

            Export(viewModel, document, navisworksExportOptions, ref logger, ref isFuckedUp);
        }
        private static NavisworksExportOptions NWC_ExportOptions(NWC_ViewModel nwc_ViewModel, Document document)
        {
            NavisworksExportOptions options = new()
            {
                ConvertElementProperties = nwc_ViewModel.ConvertElementProperties,
                DivideFileIntoLevels = nwc_ViewModel.DivideFileIntoLevels,
                ExportElementIds = nwc_ViewModel.ExportElementIds,
                ExportLinks = nwc_ViewModel.ExportLinks,
                ExportParts = nwc_ViewModel.ExportParts,
                ExportRoomAsAttribute = nwc_ViewModel.ExportRoomAsAttribute,
                ExportRoomGeometry = nwc_ViewModel.ExportRoomGeometry,
                ExportUrls = nwc_ViewModel.ExportUrls,
                FindMissingMaterials = nwc_ViewModel.FindMissingMaterials,
                ConvertLights = nwc_ViewModel.ConvertLights,
                ConvertLinkedCADFormats = nwc_ViewModel.ConvertLinkedCADFormats,
                Coordinates = nwc_ViewModel.SelectedCoordinates.Key,
                Parameters = nwc_ViewModel.SelectedParameters.Key,
                FacetingFactor = double
                    .TryParse(nwc_ViewModel.FacetingFactor, out double facetingFactor)
                    ? facetingFactor
                    : 1.0,
                ExportScope = nwc_ViewModel.ExportScopeView
                    ? NavisworksExportScope.View
                    : NavisworksExportScope.Model

            };
            if (nwc_ViewModel.ExportScopeView)
                options.ViewId = new FilteredElementCollector(document)
                        .OfClass(typeof(View3D))
                        .FirstOrDefault(e => e.Name == nwc_ViewModel.ViewName
                            && !((View3D)e).IsTemplate)
                        .Id;
            return options;
        }
    }
}