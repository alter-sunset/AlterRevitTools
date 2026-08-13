using AlterTools.atExportModel.Enums;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Configs;

public class ConfigIFC
{
    public bool UseUserMapping { get; set; } = false;
    public string FamilyMappingFile { get; set; } = string.Empty;
    public bool ExportBaseQuantities { get; set; } = true;
    public bool WallAndColumnSplitting { get; set; } = true;
    public IFCVersion FileVersion { get; set; } = IFCVersion.IFC2x3CV2;
    public SpaceBoundaryLevel SpaceBoundaryLevel { get; set; } = SpaceBoundaryLevel.None;
}