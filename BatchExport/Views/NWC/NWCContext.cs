using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.NWC;

internal static class NWCContext
{
    public static IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates { get; } =
        new Dictionary<NavisworksCoordinates, string>
        {
            { NavisworksCoordinates.Shared, Resources.Resources.NWC_Shared },
            { NavisworksCoordinates.Internal, Resources.Resources.NWC_Internal }
        };

    public static IReadOnlyDictionary<NavisworksParameters, string> Parameters { get; } =
        new Dictionary<NavisworksParameters, string>
        {
            { NavisworksParameters.All, Resources.Resources.NWC_All },
            { NavisworksParameters.Elements, Resources.Resources.NWC_Elements },
            { NavisworksParameters.None, Resources.Resources.NWC_None }
        };
}