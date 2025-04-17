using Autodesk.Revit.DB;
using AlterTools.BatchExport.Views.IFC;

namespace AlterTools.DriveFromOutside.Events.IFC
{
    public class IfcConfig : IConfigIfc
    {
        public string FamilyMappingFile { get; set; }
        public bool ExportBaseQuantities { get; set; }
        public bool WallAndColumnSplitting { get; set; }
        public IFCVersion FileVersion { get; set; }
        public int SpaceBoundaryLevel { get; set; }
        public string NamePrefix { get; set; }
        public string NamePostfix { get; set; }
        public string[] WorksetPrefixes { get; set; }
        public bool ExportScopeView { get; set; }
        public bool ExportScopeWhole { get; set; }
        public string[] Files { get; set; }
        public string ViewName { get; set; }
        public string FolderPath { get; set; }
    }
}