namespace AlterTools.Utils;

public class ParametersTable
{
    public string ModelName { get; set; }
    public long ElementId { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}