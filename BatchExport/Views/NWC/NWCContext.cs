using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.NWC;

internal static class NWCContext
{
    public static IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates { get; } =
        new Dictionary<NavisworksCoordinates, string>
        {
            { NavisworksCoordinates.Shared, Resources.Strings.NWC_Shared },
            { NavisworksCoordinates.Internal, Resources.Strings.NWC_Internal }
        };

    public static IReadOnlyDictionary<NavisworksParameters, string> Parameters { get; } =
        new Dictionary<NavisworksParameters, string>
        {
            { NavisworksParameters.All, Resources.Strings.NWC_All },
            { NavisworksParameters.Elements, Resources.Strings.NWC_Elements },
            { NavisworksParameters.None, Resources.Strings.NWC_None }
        };
}