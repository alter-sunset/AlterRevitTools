using AlterTools.Resources;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.UI;

internal static class NWCContext
{
    public static IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates { get; } =
        new Dictionary<NavisworksCoordinates, string>
        {
            { NavisworksCoordinates.Shared, Strings.Shared },
            { NavisworksCoordinates.Internal, Strings.Internal }
        };

    public static IReadOnlyDictionary<NavisworksParameters, string> Parameters { get; } =
        new Dictionary<NavisworksParameters, string>
        {
            { NavisworksParameters.All, Strings.All },
            { NavisworksParameters.Elements, Strings.Elements },
            { NavisworksParameters.None, Strings.None }
        };

    public static IReadOnlyDictionary<NavisworksExportScope, string> ExportScopes { get; } =
        new Dictionary<NavisworksExportScope, string>
        {
            { NavisworksExportScope.Model, Strings.Model },
            { NavisworksExportScope.View, Strings.View }
        };
}