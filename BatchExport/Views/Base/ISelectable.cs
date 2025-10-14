namespace AlterTools.BatchExport.Views.Base;

public interface ISelectable
{
    [UsedImplicitly]
    bool IsSelected { get; set; }
}