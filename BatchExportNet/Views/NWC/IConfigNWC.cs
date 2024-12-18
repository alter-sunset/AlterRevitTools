using Autodesk.Revit.DB;
using AlterTools.BatchExportNet.Views.Base;

namespace AlterTools.BatchExportNet.Views.NWC
{
    public interface IConfigNWC : IConfigBase_Extended
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
        public NavisworksParameters Parameters { get; }
        public NavisworksCoordinates Coordinates { get; }
    }
}