using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.Detach
{
    public interface IConfigDetach : IConfigBase
    {
        string MaskInName { get; set; }
        string MaskOutName { get; set; }
        bool IsToRename { get; set; }
        bool CheckForEmptyView { get; set; }
    }
}