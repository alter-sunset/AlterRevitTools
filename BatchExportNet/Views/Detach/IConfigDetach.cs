using AlterTools.BatchExportNet.Views.Base;

namespace AlterTools.BatchExportNet.Views.Detach
{
    public interface IConfigDetach : IConfigBase
    {
        public string MaskInName { get; set; }
        public string MaskOutName { get; set; }
        public bool RemoveLinks { get; set; }
        public bool RemoveEmptyWorksets { get; set; }
        public bool Purge { get; set; }
        public bool IsToRename { get; set; }
        public bool CheckForEmptyView { get; set; }
    }
}