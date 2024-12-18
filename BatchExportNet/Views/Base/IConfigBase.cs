namespace AlterTools.BatchExportNet.Views.Base
{
    public interface IConfigBase
    {
        public string[] Files { get; }
        public string ViewName { get; set; }
        public string FolderPath { get; set; }
    }
}