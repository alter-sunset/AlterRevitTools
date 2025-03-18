namespace DriveFromOutsideServer.Configs
{
    public class DetachConfigBase
    {
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
    public class DetachConfigEmperor : DetachConfigBase
    {
        public string[] Files { get; set; }
    }
    public class DetachConfigKing : DetachConfigBase
    {
        public string File { get; set; }
    }
}
