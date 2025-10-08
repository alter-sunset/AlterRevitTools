using System.Collections.Generic;
using AlterTools.BatchExport.Resources;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.NWC;

internal static class NWCContext
{
    public static IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates { get; } =
        new Dictionary<NavisworksCoordinates, string>
        {
            { NavisworksCoordinates.Shared, Strings.NWC_Shared },
            { NavisworksCoordinates.Internal, Strings.NWC_Internal }
        };

    public static IReadOnlyDictionary<NavisworksParameters, string> Parameters { get; } =
        new Dictionary<NavisworksParameters, string>
        {
            { NavisworksParameters.All, Strings.NWC_All },
            { NavisworksParameters.Elements, Strings.NWC_Elements },
            { NavisworksParameters.None, Strings.NWC_None }
        };
}