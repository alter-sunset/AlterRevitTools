using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace AlterTools.BatchExport.Views.NWC
{
    internal static class NWC_Context
    {
        private static readonly IReadOnlyDictionary<NavisworksCoordinates, string> _coordinates
            = new Dictionary<NavisworksCoordinates, string>()
            {
                { NavisworksCoordinates.Shared, "Общие" },
                { NavisworksCoordinates.Internal, "Внутренние для проекта" }
            };
        public static IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates => _coordinates;

        private static readonly IReadOnlyDictionary<NavisworksParameters, string> _parameters
            = new Dictionary<NavisworksParameters, string>()
            {
                { NavisworksParameters.All, "Все" },
                { NavisworksParameters.Elements, "Объекты" },
                { NavisworksParameters.None, "Нет" }
            };
        public static IReadOnlyDictionary<NavisworksParameters, string> Parameters => _parameters;
    }
}