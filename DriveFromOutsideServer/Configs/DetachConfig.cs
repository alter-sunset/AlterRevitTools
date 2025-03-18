namespace DriveFromOutsideServer.Configs
{
    public class DetachConfigBase : IConfigBase
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
    public class DetachConfigEmperor : DetachConfigBase, IConfigEmperor
    {
        public string[] Files { get; set; }
    }
    public class DetachConfigKing : DetachConfigBase, IConfigKing
    {
        public string File { get; set; }
    }
}
