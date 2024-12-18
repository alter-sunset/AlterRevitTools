using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.IFC
{
    public class IFCForm : IFCExportOptions
    {
        private string _folderPath;
        private string _namePrefix;
        private string _namePostfix;
        private string[] _worksetPrefixes;
        private string[] _files;
        private string _viewName;
        private bool _exportView;

        public string FolderPath { get => _folderPath; set => _folderPath = value; }
        public string NamePrefix { get => _namePrefix; set => _namePrefix = value; }
        public string NamePostfix { get => _namePostfix; set => _namePostfix = value; }
        public string[] WorksetPrefixes { get => _worksetPrefixes; set => _worksetPrefixes = value; }
        public string[] Files { get => _files; set => _files = value; }
        public string ViewName { get => _viewName; set => _viewName = value; }
        public bool ExportView { get => _exportView; set => _exportView = value; }
        public new bool FilterViewId { get; set; }
    }
}