using System.Collections.ObjectModel;

namespace AlterTools.atParams;

public class ConfigParams : IConfigParams
{
    public ObservableCollection<string> Files { get; set; }
    public string[] ParametersNames { get; set; }
    public string CsvPath { get; set; }
}