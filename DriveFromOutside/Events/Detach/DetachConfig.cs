using AlterTools.BatchExport.Views.Detach;

namespace AlterTools.DriveFromOutside.Events.Detach;

public class DetachConfig : IConfigDetach
{
    public bool Purge { get; set; }
    public string[] Files { get; set; } = [];
    public string FolderPath { get; set; } = string.Empty;
    public string MaskInName { get; set; } = string.Empty;
    public string MaskOutName { get; set; } = string.Empty;
    public string ViewName { get; set; } = string.Empty;
    public bool RemoveEmptyWorksets { get; set; }
    public bool IsToRename { get; set; }
    public bool CheckForEmptyView { get; set; }
    public bool RemoveLinks { get; set; }
}