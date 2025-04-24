namespace AlterTools.BatchExport.Views.Base;

public interface IConfigBaseExtended : IConfigBase
{
    string NamePrefix { get; set; }
    string NamePostfix { get; set; }
    string[] WorksetPrefixes { get; }
    bool ExportScopeView { get; }
    bool ExportScopeWhole { get; set; }
}