using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Interfaces;

public interface IConfigNWC
{
    bool ConvertElementProperties { get; set; }
    bool DivideFileIntoLevels { get; set; }
    bool ExportElementIds { get; set; }
    bool ExportLinks { get; set; }
    bool ExportParts { get; set; }
    bool ExportRoomAsAttribute { get; set; }
    bool ExportRoomGeometry { get; set; }
    bool ExportUrls { get; set; }
    bool FindMissingMaterials { get; set; }
    bool ConvertLinkedCADFormats { get; set; }
    bool ConvertLights { get; set; }
    double FacetingFactor { get; set; }
    NavisworksParameters Parameters { get; set; }
    NavisworksCoordinates Coordinates { get; set; }
}