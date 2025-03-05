using System;
using System.IO;

namespace AlterTools.BatchExport.Utils
{
    public class Logger
    {
        private const string BASE_ERROR = "Произошла срань.";
        private readonly DateTime _startTime = DateTime.Now;
        private readonly StreamWriter _stream;
        private int _successCount = 0;
        private int _errorCount = 0;
        public int ErrorCount { get => _errorCount; set => _errorCount = value; }
        public int SuccessCount { get => _successCount; set => _successCount = value; }
        public Logger(string path)
        {
            _stream = new StreamWriter($@"{path}\Log_{_startTime:yy-MM-dd_HH-mm-ss}.log");
            _stream.WriteLine($"Initial launch at {_startTime}.");
        }
        public void Error(string error, Exception ex = null)
        {
            string lineToWrite = $"Error at {DateTime.Now}. {BASE_ERROR} {error}" +
                          (ex != null ? $" {ex.Message}" : string.Empty);
            _stream.WriteLine(lineToWrite);
            ErrorCount++;
        }
        public void Success(string message) => _stream.WriteLine($"Success at {DateTime.Now}. {message}");
        public void FileOpened() => _stream.WriteLine("File successfully opened.");
        public void Start(string file) => _stream.WriteLine($"Started work at {DateTime.Now} on {file}");
        public void LineBreak() => _stream.WriteLine("--||--");
        public void TimeForFile(DateTime startTime) => _stream.WriteLine($"Time spent for file {DateTime.Now - startTime}");
        public void TimeTotal() => _stream.WriteLine($"Total time spent {DateTime.Now - _startTime}");
        public void Hash(string hash) => _stream.WriteLine($"MD5 Hash of file is: {hash} at {DateTime.Now}");
        public void ErrorTotal() => _stream.WriteLine($"Done! There were {ErrorCount} errors out of {ErrorCount + SuccessCount} files.");
        public void Dispose() => _stream.Dispose();
    }
}