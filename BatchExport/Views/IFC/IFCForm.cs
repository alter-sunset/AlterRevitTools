using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.IFC;

public class IFCForm : IFCExportOptions
{
    public string FolderPath { get; set; }
    public string NamePrefix { get; set; }
    public string NamePostfix { get; set; }
    public string[] WorksetPrefixes { get; set; }
    public string[] Files { get; set; }
    public string ViewName { get; set; }
    public bool ExportView { get; set; }
    public new bool FilterViewId { get; set; }
}