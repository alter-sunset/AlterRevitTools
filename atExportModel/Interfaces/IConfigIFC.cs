using AlterTools.atExportModel.Enums;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Interfaces;

public interface IConfigIFC
{
    string FamilyMappingFile { get; set; }
    bool ExportBaseQuantities { get; set; }
    bool WallAndColumnSplitting { get; set; }
    IFCVersion FileVersion { get; set; }
    SpaceBoundaryLevel SpaceBoundaryLevel { get; set; }
}