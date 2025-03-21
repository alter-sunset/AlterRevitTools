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

        public void InheritFromEmperor(IConfigEmperor emperor)
        {
            if (emperor is null || emperor is not UpdateConfigEmperor emp) throw new NullReferenceException();

            FolderPath = emp.FolderPath;
            VersionEnd = emp.VersionEnd;
        }
    }
}