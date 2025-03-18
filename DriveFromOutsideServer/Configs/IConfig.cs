namespace DriveFromOutsideServer.Configs
{
    public interface IConfigBase
    {
    }
    public interface IConfigEmperor : IConfigBase
    {
        public string[] Files { get; set; }
    }
    public interface IConfigKing : IConfigBase
    {
        public string File { get; set; }
    }
}
