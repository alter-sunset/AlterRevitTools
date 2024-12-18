using AlterTools.BatchExportNet.Views.Detach;

namespace AlterTools.DriveFromOutside.Events.Detach
{
    public class DetachConfig : IConfigDetach
    {
        public string[] Files { get; set; }
        public string FolderPath { get; set; }
        public string MaskInName { get; set; }
        public string MaskOutName { get; set; }
        public string ViewName { get; set; }
        public bool RemoveEmptyWorksets { get; set; }
        public bool Purge { get; set; }
        public bool IsToRename { get; set; }
        public bool CheckForEmptyView { get; set; }
        public bool RemoveLinks { get; set; }
    }
}