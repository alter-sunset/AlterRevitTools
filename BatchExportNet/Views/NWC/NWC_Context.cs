using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace VLS.BatchExportNet.Views.NWC
{
    internal static class NWC_Context
    {
        private static readonly Dictionary<NavisworksCoordinates, string> _coordinates = new()
        {
            { NavisworksCoordinates.Shared, "Общие" },
            { NavisworksCoordinates.Internal, "Внутренние для проекта" }
        };
        public static Dictionary<NavisworksCoordinates, string> Coordinates
        {
            get => _coordinates;
        }

        private static readonly Dictionary<NavisworksParameters, string> _parameters = new()
        {
            { NavisworksParameters.All, "Все" },
            { NavisworksParameters.Elements, "Объекты" },
            { NavisworksParameters.None, "Нет" }
        };
        public static Dictionary<NavisworksParameters, string> Parameters
        {
            get => _parameters;
        }
    }
}