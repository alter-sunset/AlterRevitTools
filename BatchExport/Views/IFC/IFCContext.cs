using System.Collections.Generic;
using AlterTools.BatchExport.Resources;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.IFC;

public static class IFCContext
{
    public static IReadOnlyDictionary<IFCVersion, string> IFCVersions { get; } = new Dictionary<IFCVersion, string>
    {
        { IFCVersion.Default, Strings.Const_Default },
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

    public static IReadOnlyDictionary<int, string> SpaceBoundaryLevels { get; } = new Dictionary<int, string>
    {
        { 0, Strings.IFC_SpaceBoundaryLevels0 },
        { 1, Strings.IFC_SpaceBoundaryLevels1 },
        { 2, Strings.IFC_SpaceBoundaryLevels2 }
    };
}