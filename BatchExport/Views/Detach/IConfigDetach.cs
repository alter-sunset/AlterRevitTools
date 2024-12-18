using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.Detach
{
    public interface IConfigDetach : IConfigBase
    {
        string MaskInName { get; set; }
        string MaskOutName { get; set; }
        bool IsToRename { get; set; }
        bool CheckForEmptyView { get; set; }
#if R23 || R24
        bool RemoveEmptyWorksets { get; set; }
#endif
#if R24
        bool Purge { get; set; }
#endif
    }
}