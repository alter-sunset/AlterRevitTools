using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace VLS.BatchExportNet.Views.IFC
{
    internal static class IFC_Context
    {
        private static readonly Dictionary<IFCVersion, string> _ifcVersions = new()
        {
            { IFCVersion.Default, "По умолчанию" },
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
            { IFCVersion.IFC4x3, "IFC 4x3" },
            { IFCVersion.IFCSG, "IFC-SG Regulatory Requirements View" }
        };
        public static Dictionary<IFCVersion, string> IFCVersions
        {
            get => _ifcVersions;
        }
        private static readonly Dictionary<int, string> _spaceBoundaryLevels = new()
        {
            { 0, "Нет" },
            { 1, "Первый уровень" },
            { 2, "Второй уровень" }
        };
        public static Dictionary<int, string> SpaceBoundaryLevels
        {
            get => _spaceBoundaryLevels;
        }
    }
}