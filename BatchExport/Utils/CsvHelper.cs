using AlterTools.BatchExport.Views.Params;
using System;
using System.IO;
using System.Linq;

namespace AlterTools.BatchExport.Utils
{
    public class CsvHelper : IDisposable
    {
        private readonly StreamWriter _stream;
        public CsvHelper(string csvFilePath, string[] parametersNames)
        {
            _stream = new(csvFilePath);
            _stream.WriteLine(
                string.Join(";",
                    new[] { "ModelName", "ElementId" }.Concat(parametersNames))); // Header
        }

        public void WriteElement(ParametersTable paramsTable)
            => _stream.WriteLine(
                    string.Join(";",
                        new[] { paramsTable.ModelName, paramsTable.ElementId.ToString() }.Concat(paramsTable.Parameters.Values)));

        public void Dispose() => _stream.Dispose();
    }
}