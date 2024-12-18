using Autodesk.Revit.DB;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.IFC
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