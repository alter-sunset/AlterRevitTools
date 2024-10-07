using VLS.BatchExportNet.Views.Base;

namespace VLS.DriveFromOutside.Events.Transmit
{
    public class TransmitConfig
    {
        public List<string> Files { get; set; }
        public string FolderPath { get; set; }
    }
}