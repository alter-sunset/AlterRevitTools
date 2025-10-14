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
    
    [UsedImplicitly]
    public new bool FilterViewId { get; set; }
    public bool TurnOffLog {get; set;}
}