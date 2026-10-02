using System.Collections.ObjectModel;

namespace AlterTools.atParams;

public class ConfigParams : IConfigParams
{
    public string CsvPath { get; set; }
    public string ViewName { get; set; }
    public string[] ParametersNames { get; set; }
    public ObservableCollection<string> Files { get; set; }
}