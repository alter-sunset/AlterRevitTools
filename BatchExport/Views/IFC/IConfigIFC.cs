using AlterTools.BatchExport.Views.Base;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.IFC;

public interface IConfigIFC : IConfigBaseExtended
{
    string FamilyMappingFile { get; }
    bool ExportBaseQuantities { get; }
    bool WallAndColumnSplitting { get; }
    IFCVersion FileVersion { get; }
    int SpaceBoundaryLevel { get; }
}