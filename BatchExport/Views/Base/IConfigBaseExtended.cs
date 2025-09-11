namespace AlterTools.BatchExport.Views.Base;

public interface IConfigBaseExtended : IConfigBase
{
    string NamePrefix { get; set; }
    string NamePostfix { get; set; }
    string[] WorksetPrefixes { get; }
    bool ExportScopeView { get; set; }
    bool IgnoreMissingView { get; set; }
    bool ExportScopeWhole { get; set; }
    bool TurnOffLog { get; set; }
}