namespace AlterTools.atExportModel.Interfaces;

public interface IConfigDetach
{
    string MaskInName { get; set; }
    string MaskOutName { get; set; }
    bool RemoveLinks { get; set; }
    bool IsToRename { get; set; }
    bool CheckForEmptyView { get; set; }
#if R22_OR_GREATER
    bool RemoveEmptyWorksets { get; set; }
#endif
    bool Purge { get; set; }
}