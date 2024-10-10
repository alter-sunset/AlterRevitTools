using System;
using System.IO;
using System.Windows.Forms;

namespace VLS.BatchExportNet.Utils
{
    public class Logger : IDisposable
    {
        private string _path;
        private DateTime _startTime;
        private string _fileName;
        private string _filePath;
        private int _errorCount;
        private int _successCount;
        private const string BASE_ERROR = "Произошла срань.";
        public string Path { get => _path; set => _path = value; }
        public int ErrorCount { get => _errorCount; set => _errorCount = value; }
        public int SuccessCount { get => _successCount; set => _successCount = value; }
        public Logger(string path)
        {
            Path = path;
            _startTime = DateTime.Now;
            ErrorCount = 0;
            SuccessCount = 0;
            _fileName = $"Log_{_startTime:yy-MM-dd_HH-mm-ss}.log";
            _filePath = $@"{Path}\{_fileName}";
            WriteLine($"Initial launch at {_startTime}.");
        }
        public void Error(string error, Exception ex = null)
        {
            string lineToWrite = $"Error at {DateTime.Now}. {BASE_ERROR} {error}" +
                          (ex != null ? $" {ex.Message}" : string.Empty);
            WriteLine(lineToWrite);
            ErrorCount++;
        }
        public void Success(string message)
            => WriteLine($"Success at {DateTime.Now}. {message}");
        public void FileOpened()
            => WriteLine("File successfully opened.");
        public void Start(string file)
            => WriteLine($"Started work at {DateTime.Now} on {file}");
        public void LineBreak()
            => WriteLine("--||--");
        public void TimeForFile(DateTime startTime)
            => WriteLine($"Time spent for file {DateTime.Now - startTime}");
        public void TimeTotal()
            => WriteLine($"Total time spent {DateTime.Now - _startTime}");
        public void Hash(string hash)
            => WriteLine($"MD5 Hash of file is: {hash} at {DateTime.Now}");
        public void ErrorTotal()
            => WriteLine($"Done! There were {ErrorCount} errors out of {ErrorCount + SuccessCount} files.");
        private void WriteLine(string lineToWrite)
        {
            try
            {
                File.AppendAllText(_filePath, $"{lineToWrite}\n");
            }
            catch
            {
                MessageBox.Show("Проблемы с файлом логов");
            }
        }
        public void Dispose()
        {
            // Clean up if necessary, but for now just clear properties
            _path = null;
            _fileName = null;
            _filePath = null;
            ErrorCount = 0;
            SuccessCount = 0;
        }
    }
}