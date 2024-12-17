namespace VLS.BatchExport.Views.Base
{
    public interface IConfigBase_Extended : IConfigBase
    {
        string NamePrefix { get; set; }
        string NamePostfix { get; set; }
        string[] WorksetPrefixes { get; }
        bool ExportScopeView { get; }
        bool ExportScopeWhole { get; set; }
    }
}