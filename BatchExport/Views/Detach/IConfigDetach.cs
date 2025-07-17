using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Detach;

public interface IConfigDetach : IConfigBase
{
    string MaskInName { get; set; }
    string MaskOutName { get; set; }
    bool RemoveLinks { get; set; }
    bool IsToRename { get; set; }
    bool CheckForEmptyView { get; set; }

#if R22_OR_GREATER
    bool RemoveEmptyWorksets { get; set; }
#endif

#if R24_OR_GREATER
    bool Purge { get; set; }
#endif
}