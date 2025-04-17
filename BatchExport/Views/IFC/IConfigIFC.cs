using Autodesk.Revit.DB;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.IFC
{
    public interface IConfigIFC : IConfigBaseExtended
    {
        string FamilyMappingFile { get; }
        bool ExportBaseQuantities { get; }
        bool WallAndColumnSplitting { get; }
        IFCVersion FileVersion { get; }
        int SpaceBoundaryLevel { get; }
    }
}