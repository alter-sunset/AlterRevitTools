using System.Collections.ObjectModel;

namespace AlterTools.atParams;

public interface IConfigParams
{
    public string CsvPath { get; set; }
    public string[] ParametersNames { get; set; }
    public ObservableCollection<string> Files { get; set; }
}