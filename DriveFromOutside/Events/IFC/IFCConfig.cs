using AlterTools.BatchExport.Views.IFC;
using Autodesk.Revit.DB;

namespace AlterTools.DriveFromOutside.Events.IFC;

public class IFCConfig : IConfigIFC
{
    public string FamilyMappingFile { get; set; } = string.Empty;
    public bool ExportBaseQuantities { get; set; }
    public bool WallAndColumnSplitting { get; set; }
    public IFCVersion FileVersion { get; set; }
    public int SpaceBoundaryLevel { get; set; }
    public string NamePrefix { get; set; } = string.Empty;
    public string NamePostfix { get; set; } = string.Empty;
    public string[] WorksetPrefixes { get; set; } = [];
    public bool ExportScopeView { get; set; }
    public bool ExportScopeWhole { get; set; }
    public string[] Files { get; set; } = [];
    public string ViewName { get; set; } = string.Empty;
    public string FolderPath { get; set; } = string.Empty;
}