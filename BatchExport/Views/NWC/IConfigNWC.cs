using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.NWC;

public interface IConfigNWC : IConfigBaseExtended
{
    [UsedImplicitly]
    bool ConvertElementProperties { get; set; }
    
    [UsedImplicitly]
    bool DivideFileIntoLevels { get; set; }
    
    [UsedImplicitly]
    bool ExportElementIds { get; set; }
    
    [UsedImplicitly]
    bool ExportLinks { get; set; }
    
    [UsedImplicitly]
    bool ExportParts { get; set; }
    
    [UsedImplicitly]
    bool ExportRoomAsAttribute { get; set; }
    
    [UsedImplicitly]
    bool ExportRoomGeometry { get; set; }
    
    [UsedImplicitly]
    bool ExportUrls { get; set; }
    
    [UsedImplicitly]
    bool FindMissingMaterials { get; set; }
    
    [UsedImplicitly]
    bool ConvertLinkedCADFormats { get; set; }
    
    [UsedImplicitly]
    bool ConvertLights { get; set; }
    
    [UsedImplicitly]
    double FacetingFactor { get; set; }
    
    NavisworksParameters Parameters { get; }
    NavisworksCoordinates Coordinates { get; }
}