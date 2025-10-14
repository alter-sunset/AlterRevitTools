using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Params;

public interface IConfigParams : IConfigBase
{
    [UsedImplicitly]
    public string[] ParametersNames { get; }
    [UsedImplicitly]
    public string CsvPath { get; set; }
}