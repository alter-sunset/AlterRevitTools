using System;
using System.IO;
using System.Linq;
using AlterTools.BatchExport.Views.Params;

namespace AlterTools.BatchExport.Utils
{
    public class CsvHelper : IDisposable
    {
        private static readonly string[] HeaderBase = ["ModelName", "ElementId"];
        private readonly StreamWriter _stream;

        public CsvHelper(string csvFilePath, string[] parametersNames)
        {
            _stream = new StreamWriter(csvFilePath);
            _stream.WriteLine(string.Join("|", HeaderBase.Concat(parametersNames))); // Header
        }

        public CsvHelper(string csvFilePath)
        {
            _stream = new StreamWriter(csvFilePath);
            _stream.WriteLine("ModelName|WorksetName");
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public void WriteElement(ParametersTable paramsTable)
        {
            _stream.WriteLine(string.Join("|",
                new[] { paramsTable.ModelName, paramsTable.ElementId.ToString() }
                    .Concat(paramsTable.Parameters.Values)));
        }

        public void WriteWorkset(string modelName, string worksetName)
        {
            _stream.WriteLine($"{modelName}|{worksetName}");
        }
    }
}