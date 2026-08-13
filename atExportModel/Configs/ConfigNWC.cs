using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Configs;

public class ConfigNWC
{
    public bool ConvertElementProperties { get; set; } = true;
    public bool DivideFileIntoLevels { get; set; } = true;
    public bool ExportElementIds { get; set; } = true;
    public bool ExportLinks { get; set; } = false;
    public bool ExportParts { get; set; } = true;
    public bool ExportRoomAsAttribute { get; set; } = true;
    public bool ExportRoomGeometry { get; set; } = false;
    public bool ExportUrls { get; set; } = false;
    public bool FindMissingMaterials { get; set; } = true;
    public bool ConvertLinkedCADFormats { get; set; } = false;
    public bool ConvertLights { get; set; } = true;
    public double FacetingFactor { get; set; } = 1.0;
    public NavisworksParameters Parameters { get; set; } = NavisworksParameters.All;
    public NavisworksCoordinates Coordinates { get; set; } = NavisworksCoordinates.Shared;
    public NavisworksExportScope ExportScope { get; set; } = NavisworksExportScope.View;
}