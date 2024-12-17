using Autodesk.Revit.DB;
using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.IFC
{
    public interface IConfigIFC : IConfigBase_Extended
    {
        string FamilyMappingFile { get; }
        bool ExportBaseQuantities { get; set; }
        bool WallAndColumnSplitting { get; set; }
        IFCVersion FileVersion { get; }
        int SpaceBoundaryLevel { get; }
    }
}