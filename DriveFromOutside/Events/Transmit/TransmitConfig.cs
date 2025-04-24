namespace AlterTools.DriveFromOutside.Events.Transmit;

public abstract class TransmitConfig
{
    public string[] Files { get; set; } = [];
    public string FolderPath { get; set; } = string.Empty;
}