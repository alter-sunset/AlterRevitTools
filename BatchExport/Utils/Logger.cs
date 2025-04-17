using System;
using System.IO;

namespace AlterTools.BatchExport.Utils
{
    public class Logger
    {
        private const string BaseError = "ERROR";

        private readonly DateTime _startTime = DateTime.Now;
        private readonly StreamWriter _stream;

        public int ErrorCount { get; private set; }
        public int SuccessCount { get; set; } = 0;

        public Logger(string path)
        {
            _stream = new StreamWriter($@"{path}\Log_{_startTime:yy-MM-dd_HH-mm-ss}.log");
            _stream.WriteLine($"Initial launch at {_startTime}.");
        }

        public void Error(string error, Exception ex = null)
        {
            string lineToWrite = $"Error at {DateTime.Now}. {BaseError} {error}" +
                                 (ex != null ? $" {ex.Message}" : string.Empty);
            _stream.WriteLine(lineToWrite);
            ErrorCount++;
        }

        public void Start(string file) => _stream.WriteLine($"Started work at {DateTime.Now} on {file}");
        public void FileOpened() => _stream.WriteLine("File successfully opened.");

        public void Success(string message) => _stream.WriteLine($"Success at {DateTime.Now}. {message}");
        public void LineBreak() => _stream.WriteLine("--||--");
        public void Hash(string hash) => _stream.WriteLine($"MD5 Hash of file is: {hash} at {DateTime.Now}");

        public void TimeForFile(DateTime startTime) => _stream.WriteLine($"Time spent for file {DateTime.Now - startTime}");
        public void TimeTotal() => _stream.WriteLine($"Total time spent {DateTime.Now - _startTime}");
        public void ErrorTotal() => _stream.WriteLine($"Done! There were {ErrorCount} errors out of {ErrorCount + SuccessCount} files.");

        public void Dispose() => _stream.Dispose();
    }
}