using AlterTools.BatchExport.Views.Params;
using System;
using System.IO;
using System.Linq;

namespace AlterTools.BatchExport.Utils
{
    public class CsvHelper : IDisposable
    {
        private static readonly string[] _headerBase = new string[] { "ModelName", "ElementId" };
        private readonly StreamWriter _stream;

        public CsvHelper(string csvFilePath, string[] parametersNames)
        {
            _stream = new(csvFilePath);
            _stream.WriteLine(string.Join("|", _headerBase.Concat(parametersNames))); // Header
        }

        public CsvHelper(string csvFilePath)
        {
            _stream = new(csvFilePath);
            _stream.WriteLine("ModelName|WorksetName");
        }

        public void WriteElement(ParametersTable paramsTable)
        {
            _stream.WriteLine(string.Join("|", new string[] { paramsTable.ModelName, paramsTable.ElementId.ToString() }.Concat(paramsTable.Parameters.Values)));
        }

        public void WriteWorkset(string modelName, string worksetName) => _stream.WriteLine($"{modelName}|{worksetName}");

        public void Dispose() => _stream.Dispose();
    }
}