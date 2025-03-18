namespace DriveFromOutsideServer.Configs
{
    public class IfcConfigBase
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
        public string ViewName { get; set; }
        public string FolderPath { get; set; }
    }
    public class IfcConfigEmperor : IfcConfigBase
    {
        public string[] Files { get; set; }
    }
    public class IfcConfigKing : IfcConfigBase
    {
        public string File { get; set; }
    }
    public enum IFCVersion
    {
        Default = 0,
        IFC2x2 = 9,
        IFC2x3 = 10,
        IFCBCA = 8,
        IFC2x3CV2 = 21,
        IFCCOBIE = 17,
        IFC4 = 23,
        IFC2x3FM = 24,
        IFC4RV = 25,
        IFC4DTV = 26,
        IFC2x3BFM = 27,
        IFC4x3 = 29,
        IFCSG = 30
    }
}