using Autodesk.Revit.DB;

namespace AlterTools.BatchExportNet.Views.NWC
{
    public class NWCForm : NavisworksExportOptions
    {
        private string _folderPath;
        private string _namePrefix;
        private string _namePostfix;
        private string[] _worksetPrefixes;
        private string[] _files;
        private string _viewName;
        private bool _convertLights;
        private bool _convertLinkedCADFormats;
        private double _facetingFactor;

        public string FolderPath { get => _folderPath; set => _folderPath = value; }
        public string NamePrefix { get => _namePrefix; set => _namePrefix = value; }
        public string NamePostfix { get => _namePostfix; set => _namePostfix = value; }
        public string[] WorksetPrefixes { get => _worksetPrefixes; set => _worksetPrefixes = value; }
        public string[] Files { get => _files; set => _files = value; }
        public string ViewName { get => _viewName; set => _viewName = value; }
        public new bool ConvertLights { get => _convertLights; set => _convertLights = value; }
        public new bool ConvertLinkedCADFormats { get => _convertLinkedCADFormats; set => _convertLinkedCADFormats = value; }
        public new double FacetingFactor { get => _facetingFactor; set => _facetingFactor = value; }
        public new bool ViewId { get; set; }
    }
}