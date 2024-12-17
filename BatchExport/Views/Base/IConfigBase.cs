namespace VLS.BatchExport.Views.Base
{
    public interface IConfigBase
    {
        string[] Files { get; }
        string ViewName { get; set; }
        string FolderPath { get; set; }
    }
}