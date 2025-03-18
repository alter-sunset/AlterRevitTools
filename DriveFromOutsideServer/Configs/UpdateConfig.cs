namespace DriveFromOutsideServer.Configs
{
    public class UpdateConfigBase
    {
        public string FolderPath { get; set; }
        public int VersionStart { get; set; }
        public int VersionEnd { get; set; }
    }
    public class UpdateConfigEmperor : UpdateConfigBase
    {
        public string[] Files { get; set; }
    }
    public class UpdateConfigKing : UpdateConfigBase
    {
        public string File { get; set; }
    }
}