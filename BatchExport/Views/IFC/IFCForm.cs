using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.IFC
{
    public class IfcForm : IFCExportOptions
    {
        public string FolderPath { get; init; }
        public string NamePrefix { get; init; }
        public string NamePostfix { get; init; }
        public string[] WorksetPrefixes { get; init; }
        public string[] Files { get; init; }
        public string ViewName { get; init; }
        public bool ExportView { get; init; }
        public new bool FilterViewId { get; set; }
    }
}