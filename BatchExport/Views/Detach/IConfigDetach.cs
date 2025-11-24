using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Detach;

public interface IConfigDetach : IConfigBase
{
    [UsedImplicitly] string MaskInName { get; set; }

    [UsedImplicitly] string MaskOutName { get; set; }

    [UsedImplicitly] bool RemoveLinks { get; set; }

    [UsedImplicitly] bool IsToRename { get; set; }

    [UsedImplicitly] bool CheckForEmptyView { get; set; }

#if R22_OR_GREATER
    [UsedImplicitly] bool RemoveEmptyWorksets { get; set; }
#endif

    [UsedImplicitly] bool Purge { get; set; }
}