using System.Collections.Generic;

namespace AlterTools.BatchExport.Views.Params;

public class ParametersTable
{
    public string ModelName { get; set; }
    public long ElementId { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}