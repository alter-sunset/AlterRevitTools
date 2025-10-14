namespace AlterTools.BatchExport.Views.Base;

public interface IConfigBase
{
    string[] Files { get; }
    
    [UsedImplicitly]
    string ViewName { get; set; }
    
    [UsedImplicitly]
    string FolderPath { get; set; }
}