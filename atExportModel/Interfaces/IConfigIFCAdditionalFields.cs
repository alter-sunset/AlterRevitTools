using AlterTools.atExportModel.Enums;

namespace AlterTools.atExportModel.Interfaces;

public interface IConfigIFCAdditionalFields
{
    ExchangeRequirement ExchangeRequirement { get; set; }
    IFCFileType IFCFileType { get; set; }
    SitePlacement SitePlacement { get; set; }
    bool IncludeSteelElements { get; set; }
    double TessellationLevelOfDetail { get; set; } // Can be between 0 and 1
    bool Export2DElements { get; set; }
    bool ExportLinkedFiles { get; set; }
    bool VisibleElementsOfCurrentView { get; set; }
    bool ExportRoomsInView { get; set; }
    bool ExportInternalRevitPropertySets { get; set; }
    bool ExportIFCCommonPropertySets { get; set; }
    bool ExportMaterialPsets { get; set; }
    bool ExportSchedulesAsPsets { get; set; }
    bool ExportSpecificSchedules { get; set; }
    bool ExportUserDefinedPsets { get; set; }
    string ExportUserDefinedPsetsFileName { get; set; }
    IClassificationSettings ClassificationSettings { get; set; } // not sure how to include it yet
    bool ExportPartsAsBuildingElements { get; set; }
    bool ExportSolidModelRep { get; set; }
    bool UseActiveViewGeometry { get; set; }
    bool UseFamilyAndTypeNameForReference { get; set; }
    bool Use2DRoomBoundaryForVolume { get; set; }
    bool IncludeSiteElevation { get; set; }

    // ReSharper disable once InconsistentNaming
    bool StoreIFCGUID { get; set; }
    bool ExportBoundingBox { get; set; }
    bool UseOnlyTriangulation { get; set; }
    bool UseTypeNameOnlyForIfcType { get; set; }
    bool UseVisibleRevitNameAsEntityName { get; set; }
}