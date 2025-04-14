using AlterTools.BatchExport.Views.Params;
using System;
using System.IO;

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
                    "ModelName",
                    "ElementId",
                    parametersNames)); // Header
        }

        public void WriteElement(ParametersTable parametersTable)
            => _stream.WriteLine(
                    string.Join(";",
                        parametersTable.ModelName,
                        parametersTable.ElementId,
                        parametersTable.Parameters.Values));

        public void Dispose() => _stream.Dispose();
    }
}