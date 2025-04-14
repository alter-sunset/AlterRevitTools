using System.Collections.Generic;

namespace AlterTools.BatchExport.Views.Params
{
    public class ParametersTable
    {
        public string ModelName { get; set; }
        public int ElementId { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}