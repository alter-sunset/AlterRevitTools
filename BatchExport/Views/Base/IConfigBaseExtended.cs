namespace AlterTools.BatchExport.Views.Base;

public interface IConfigBaseExtended : IConfigBase
{
    [UsedImplicitly]
    string NamePrefix { get; set; }
    
    [UsedImplicitly]
    string NamePostfix { get; set; }
    
    string[] WorksetPrefixes { get; }
    
    [UsedImplicitly]
    bool ExportScopeView { get; set; }
    
    [UsedImplicitly]
    bool IgnoreMissingView { get; set; }
    
    [UsedImplicitly]
    bool ExportScopeWhole { get; set; }
    
    [UsedImplicitly]
    bool TurnOffLog { get; set; }
}