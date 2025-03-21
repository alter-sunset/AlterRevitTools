namespace DriveFromOutsideServer.Configs
{
    public class UpdateConfigBase : IConfigBase
    {
        public string FolderPath { get; set; }
        public int VersionEnd { get; set; }
    }
    public class UpdateConfigEmperor : UpdateConfigBase, IConfigEmperor
    {
        public string[] Files { get; set; }
    }
    public class UpdateConfigKing : UpdateConfigBase, IConfigKing
    {
        public string File { get; set; }
    }
}