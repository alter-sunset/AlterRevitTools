namespace DriveFromOutsideServer.Configs
{
    public class NwcConfigBase : IConfigBase
    {
        public bool ConvertElementProperties { get; set; }
        public bool DivideFileIntoLevels { get; set; }
        public bool ExportElementIds { get; set; }
        public bool ExportLinks { get; set; }
        public bool ExportParts { get; set; }
        public bool ExportRoomAsAttribute { get; set; }
        public bool ExportRoomGeometry { get; set; }
        public bool ExportUrls { get; set; }
        public bool FindMissingMaterials { get; set; }
        public bool ConvertLinkedCADFormats { get; set; }
        public bool ConvertLights { get; set; }
        public double FacetingFactor { get; set; }
        public NavisworksParameters Parameters { get; set; }
        public NavisworksCoordinates Coordinates { get; set; }
        public string NamePrefix { get; set; }
        public string NamePostfix { get; set; }
        public string[] WorksetPrefixes { get; set; }
        public bool ExportScopeView { get; set; }
        public bool ExportScopeWhole { get; set; }
        public string ViewName { get; set; }
        public string FolderPath { get; set; }
    }
    public class NwcConfigEmperor : NwcConfigBase, IConfigEmperor
    {
        public string[] Files { get; set; }
    }
    public class NwcConfigKing : NwcConfigBase, IConfigKing
    {
        public string File { get; set; }

        public void InheritFromEmperor(IConfigEmperor emperor)
        {
            if (emperor is null || emperor is not NwcConfigEmperor emp) throw new NullReferenceException();

            ConvertElementProperties = emp.ConvertElementProperties;
            DivideFileIntoLevels = emp.DivideFileIntoLevels;
            ExportElementIds = emp.ExportElementIds;
            ExportLinks = emp.ExportLinks;
            ExportParts = emp.ExportParts;
            ExportRoomAsAttribute = emp.ExportRoomAsAttribute;
            ExportRoomGeometry = emp.ExportRoomGeometry;
            ExportUrls = emp.ExportUrls;
            FindMissingMaterials = emp.FindMissingMaterials;
            ConvertLinkedCADFormats = emp.ConvertLinkedCADFormats;
            ConvertLights = emp.ConvertLights;
            FacetingFactor = emp.FacetingFactor;
            Parameters = emp.Parameters;
            Coordinates = emp.Coordinates;
            NamePrefix = emp.NamePrefix;
            NamePostfix = emp.NamePostfix;
            WorksetPrefixes = emp.WorksetPrefixes;
            ExportScopeView = emp.ExportScopeView;
            ExportScopeWhole = emp.ExportScopeWhole;
            ViewName = emp.ViewName;
            FolderPath = emp.FolderPath;
        }
    }
    public enum NavisworksParameters
    {
        None,
        Elements,
        All
    }
    public enum NavisworksCoordinates
    {
        Internal,
        Shared
    }
}