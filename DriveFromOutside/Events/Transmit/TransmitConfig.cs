namespace AlterTools.DriveFromOutside.Events.Transmit
{
    public abstract class TransmitConfig
    {
        public required string[] Files { get; set; }
        public required string FolderPath { get; set; }
    }
}