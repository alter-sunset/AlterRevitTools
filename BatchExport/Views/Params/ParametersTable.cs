using System.Collections.Generic;

namespace AlterTools.BatchExport.Views.Params
{
    public class ParametersTable
    {
        public string ModelName { get; init; }
        public long ElementId { get; init; }
        public Dictionary<string, string> Parameters { get; init; }
    }
}