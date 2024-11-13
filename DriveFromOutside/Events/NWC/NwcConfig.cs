using Autodesk.Revit.DB;
using VLS.BatchExportNet.Views.NWC;

namespace VLS.DriveFromOutside.Events.NWC
{
    public class NwcConfig : IConfigNWC
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
        public string[] Files { get; set; }
        public string ViewName { get; set; }
        public string FolderPath { get; set; }
    }
}