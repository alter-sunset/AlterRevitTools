using AlterTools.atExportModel.Enums;
using AlterTools.Resources;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.UI;

internal static class ContextIFC
{
    public static IReadOnlyDictionary<IFCVersion, string> IFCVersions { get; } = new Dictionary<IFCVersion, string>
    {
        { IFCVersion.Default, Strings.Default },
        { IFCVersion.IFCBCA, "IFC 2x2 Singapore BCA e-Plan Check" },
        { IFCVersion.IFC2x2, "IFC 2x2 Coordination View" },
        { IFCVersion.IFC2x3, "IFC 2x3 Coordination View" },
        { IFCVersion.IFCCOBIE, "IFC 2x3 GSA Concept Design Bim 2010" },
        { IFCVersion.IFC2x3CV2, "IFC 2x3 Coordination View 2.0" },
        { IFCVersion.IFC2x3FM, "IFC 2x3 Extended FM Handover View" },
        { IFCVersion.IFC2x3BFM, "IFC 2x3 Basic FM Handover View" },
        { IFCVersion.IFC4, "IFC 4" },
        { IFCVersion.IFC4RV, "IFC 4 Reference View" },
        { IFCVersion.IFC4DTV, "IFC 4 Design Transfer View" },
#if R24_OR_GREATER
        { IFCVersion.IFC4x3, "IFC 4x3" },
        { IFCVersion.IFCSG, "IFC-SG Regulatory Requirements View" }
#endif
    };

    public static IReadOnlyDictionary<SpaceBoundaryLevel, string> SpaceBoundaryLevels { get; }
        = new Dictionary<SpaceBoundaryLevel, string>
        {
            { SpaceBoundaryLevel.None, Strings.None },
            { SpaceBoundaryLevel.FirstLevel, Strings.FirstLevel },
            { SpaceBoundaryLevel.SecondLevel, Strings.SecondLevel }
        };

    public static IReadOnlyDictionary<IFCFileType, string> IFCFileTypes { get; }
        = new Dictionary<IFCFileType, string>
        {
            { IFCFileType.IFC, "IFC" },
            { IFCFileType.CompressedIFC, "CompressedIFC" },
            { IFCFileType.IFCXML, "IFCXML" },
            { IFCFileType.CompressedIFCXML, "CompressedIFCXML" }
        };

    public static IReadOnlyDictionary<SitePlacement, string> SitePlacements { get; } =
        new Dictionary<SitePlacement, string>
        {
            { SitePlacement.SharedCoordinates, "Shared Coordinates" },
            { SitePlacement.SurveyPoint, "Survey Point" },
            { SitePlacement.ProjectBasePoint, "Project BasePoint" },
            { SitePlacement.InternalOrigin, "Internal Origin" },
            { SitePlacement.ProjectBasePointTrueNorth, "Project BasePoint TrueNorth" },
            { SitePlacement.InternalOriginTrueNorth, "Internal Origin TrueNorth" }
        };

    public static IReadOnlyDictionary<double, string> LevelsOfDetail { get; } = new Dictionary<double, string>
    {
        { 0.25, "Ultra-Low" },
        { 0.5, "Low" },
        { 0.75, "Medium" },
        { 1.0, "High" },
    };
}