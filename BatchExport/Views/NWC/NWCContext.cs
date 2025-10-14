using AlterTools.BatchExport.Resources;

namespace AlterTools.BatchExport.Views.NWC;

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
}