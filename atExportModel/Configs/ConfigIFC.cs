using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Configs;

public class ConfigIFC : IConfigIFC, IConfigIFCAdditionalFields
{
    public string FamilyMappingFile { get; set; }
    public bool ExportBaseQuantities { get; set; }
    public bool WallAndColumnSplitting { get; set; }
    public IFCVersion FileVersion { get; set; }
    public SpaceBoundaryLevel SpaceBoundaryLevel { get; set; }

    public ExchangeRequirement ExchangeRequirement { get; set; }
    public IFCFileType IFCFileType { get; set; }
    public SitePlacement SitePlacement { get; set; }
    public bool IncludeSteelElements { get; set; }
    public double TessellationLevelOfDetail { get; set; }
    public bool Export2DElements { get; set; }
    public bool ExportLinkedFiles { get; set; }
    public bool VisibleElementsOfCurrentView { get; set; }
    public bool ExportRoomsInView { get; set; }
    public bool ExportInternalRevitPropertySets { get; set; }
    public bool ExportIFCCommonPropertySets { get; set; }
    public bool ExportMaterialPsets { get; set; }
    public bool ExportSchedulesAsPsets { get; set; }
    public bool ExportSpecificSchedules { get; set; }
    public bool ExportUserDefinedPsets { get; set; }
    public string ExportUserDefinedPsetsFileName { get; set; }
    public IClassificationSettings ClassificationSettings { get; set; }
    public bool ExportPartsAsBuildingElements { get; set; }
    public bool ExportSolidModelRep { get; set; }
    public bool UseActiveViewGeometry { get; set; }
    public bool UseFamilyAndTypeNameForReference { get; set; }
    public bool Use2DRoomBoundaryForVolume { get; set; }
    public bool IncludeSiteElevation { get; set; }
    public bool StoreIFCGUID { get; set; }
    public bool ExportBoundingBox { get; set; }
    public bool UseOnlyTriangulation { get; set; }
    public bool UseTypeNameOnlyForIfcType { get; set; }
    public bool UseVisibleRevitNameAsEntityName { get; set; }
}