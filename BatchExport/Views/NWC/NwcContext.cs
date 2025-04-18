using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace AlterTools.BatchExport.Views.NWC
{
    internal static class NwcContext
    {
        public static IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates { get; } = new Dictionary<NavisworksCoordinates, string>()
        {
            { NavisworksCoordinates.Shared,     "Общие" },
            { NavisworksCoordinates.Internal,   "Внутренние для проекта" }
        };

        public static IReadOnlyDictionary<NavisworksParameters, string> Parameters { get; } = new Dictionary<NavisworksParameters, string>()
        {
            { NavisworksParameters.All,         "Все" },
            { NavisworksParameters.Elements,    "Объекты" },
            { NavisworksParameters.None,        "Нет" }
        };
    }
}