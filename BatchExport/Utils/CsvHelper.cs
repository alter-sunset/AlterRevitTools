using AlterTools.BatchExport.Views.Params;

namespace AlterTools.BatchExport.Utils;

public class CsvHelper : IDisposable
{
    private readonly StreamWriter _stream;
    
    public CsvHelper(string csvFilePath, string[] headers)
    {
        _stream = new StreamWriter(csvFilePath);
        _stream.WriteLine(string.Join("|", headers));
    }

    public void Dispose() => _stream.Dispose();

    public void WriteElement(ParametersTable paramsTable)
    {
        _stream.WriteLine(string.Join("|",
            new[] { paramsTable.ModelName, paramsTable.ElementId.ToString() }
                .Concat(paramsTable.Parameters
                    .Values
                    .Select(v => v.Replace(Environment.NewLine, " ")))));
    }

    public void WriteWorkset(string modelName, string worksetName) => _stream.WriteLine($"{modelName}|{worksetName}");
}