using VLS.BatchExportNet.Views.Detach;

namespace VLS.DriveFromOutside.Events.Detach
{
    public class DetachConfig : IConfigDetach
    {
        public List<string> Files { get; set; }
        public string FolderPath { get; set; }
        public string MaskInName { get; set; }
        public string MaskOutName { get; set; }
        public string ViewName { get; set; }
        public bool RemoveEmptyWorksets { get; set; }
        public bool Purge { get; set; }
        public bool IsToRename { get; set; }
        public bool CheckForEmptyView { get; set; }
    }
}