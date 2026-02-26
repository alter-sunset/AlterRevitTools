using AlterTools.BatchExport.Views.Params;

namespace AlterTools.BatchExport.Utils;

public class CsvHelper : IDisposable
{
    private readonly StreamWriter _stream;
    private readonly string _separator;

    /// <param name="csvFilePath">Path of the output file</param>
    /// <param name="headers">Array of headers</param>
    /// <param name="separator">Char that will be used as a separator. Default is |</param>
    public CsvHelper(string csvFilePath, string[] headers, char separator = '|')
    {
        _separator = separator.ToString();
        _stream = new StreamWriter(csvFilePath);
        _stream.WriteLine(string.Join(_separator, headers));
    }

    public void Dispose() => _stream.Dispose();

    public void WriteElement(ParametersTable paramsTable)
    {
        string[] start = [ paramsTable.ModelName, paramsTable.ElementId.ToString() ];
        IEnumerable<string> line = start.Concat(
            paramsTable.Parameters
                .Values
                .Select(v => v.Replace(Environment.NewLine, " ")));
        _stream.WriteLine(string.Join(_separator,  line));
    }

    public void WriteWorkset(string modelName, string worksetName)
    {
        _stream.WriteLine($"{modelName}{_separator}{worksetName}");
    }
}