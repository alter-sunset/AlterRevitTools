using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace VLS.BatchExportNet.Views.NWC
{
    public class NWCForm : NavisworksExportOptions
    {
        private string _destinationFolder;
        private string _namePrefix;
        private string _namePostfix;
        private string _worksetPrefix;
        private List<string> _rvtFiles;
        private string _viewName;
        private bool _convertLights;
        private bool _convertLinkedCADFormats;
        private double _facetingFactor;

        public string DestinationFolder { get => _destinationFolder; set => _destinationFolder = value; }
        public string NamePrefix { get => _namePrefix; set => _namePrefix = value; }
        public string NamePostfix { get => _namePostfix; set => _namePostfix = value; }
        public string WorksetPrefix { get => _worksetPrefix; set => _worksetPrefix = value; }
        public List<string> RVTFiles { get => _rvtFiles; set => _rvtFiles = value; }
        public string ViewName { get => _viewName; set => _viewName = value; }
        public new bool ConvertLights { get => _convertLights; set => _convertLights = value; }
        public new bool ConvertLinkedCADFormats { get => _convertLinkedCADFormats; set => _convertLinkedCADFormats = value; }
        public new double FacetingFactor { get => _facetingFactor; set => _facetingFactor = value; }
        public new bool ViewId { get; set; }
    }
}