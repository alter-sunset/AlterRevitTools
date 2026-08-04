using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Configs;

public class ConfigIFC : IConfigIFC, IConfigIFCAdditionalFields
{
    public bool UseUserMapping { get; set; } = false;
    public string FamilyMappingFile { get; set; } = string.Empty;
    public bool ExportBaseQuantities { get; set; } = true;
    public bool WallAndColumnSplitting { get; set; } = true;
    public IFCVersion FileVersion { get; set; } = IFCVersion.IFC2x3CV2;
    public SpaceBoundaryLevel SpaceBoundaryLevel { get; set; } = SpaceBoundaryLevel.None;
    public ExchangeRequirement ExchangeRequirement { get; set; }
    public IFCFileType IFCFileType { get; set; } = IFCFileType.IFC;
    public SitePlacement SitePlacement { get; set; } = SitePlacement.SharedCoordinates;
    public bool IncludeSteelElements { get; set; } = true;
    public double TessellationLevelOfDetail { get; set; } = 0.25;
    public bool Export2DElements { get; set; } = false;
    public bool ExportLinkedFiles { get; set; } = false;
    public bool VisibleElementsOfCurrentView { get; set; } = true;
    public bool ExportRoomsInView { get; set; } = false;
    public bool ExportInternalRevitPropertySets { get; set; } = true;
    public bool ExportIFCCommonPropertySets { get; set; } = true;
    public bool ExportMaterialPsets { get; set; } = true;
    public bool ExportSchedulesAsPsets { get; set; } = false;
    public bool ExportSpecificSchedules { get; set; } = false;
    public bool ExportUserDefinedPsets { get; set; } = false;
    public string ExportUserDefinedPsetsFileName { get; set; } = string.Empty;

    // TODO: add default classification
    //public IClassificationSettings ClassificationSettings { get; set; }

    public bool ExportPartsAsBuildingElements { get; set; } = true;
    public bool ExportSolidModelRep { get; set; } = true;
    public bool UseActiveViewGeometry { get; set; } = true;
    public bool UseFamilyAndTypeNameForReference { get; set; } = true;
    public bool Use2DRoomBoundaryForVolume { get; set; } = true;
    public bool IncludeSiteElevation { get; set; } = true;
    public bool StoreIFCGUID { get; set; } = true;
    public bool ExportBoundingBox { get; set; } = false;
    public bool UseOnlyTriangulation { get; set; } = true;
    public bool UseTypeNameOnlyForIfcType { get; set; } = false;
    public bool UseVisibleRevitNameAsEntityName { get; set; } = true;
}