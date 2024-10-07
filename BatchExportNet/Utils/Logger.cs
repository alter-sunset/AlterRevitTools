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
        private DateTime StartTime { get => _startTime; set => _startTime = value; }
        private string FileName { get => _fileName; set => _fileName = value; }
        private string FilePath { get => _filePath; set => _filePath = value; }
        public int ErrorCount { get => _errorCount; set => _errorCount = value; }
        public int SuccessCount { get => _successCount; set => _successCount = value; }
        public Logger(string path)
        {
            Path = path;
            StartTime = DateTime.Now;
            ErrorCount = 0;
            SuccessCount = 0;
            FileName = $"Log_{StartTime:yy-MM-dd_HH-mm-ss}.log";
            FilePath = $@"{Path}\{FileName}";
            string lineToWrite = $"Initial launch at {StartTime}.";
            File.WriteAllText(FilePath, lineToWrite);
        }
        public void Error(string error, Exception ex)
        {
            string lineToWrite = $"Error at {DateTime.Now}. {BASE_ERROR} {error} {ex.Message}";
            LineWriter(lineToWrite);
            ErrorCount++;
        }
        public void Error(string error)
        {
            string lineToWrite = $"Error at {DateTime.Now}. {BASE_ERROR} {error}";
            try
            {
                LineWriter(lineToWrite);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ErrorCount++;
        }
        public void Success(string success)
            => LineWriter($"Success at {DateTime.Now}. {success}");
        public void FileOpened()
            => LineWriter("File succesfully opened.");
        public void Start(string file)
            => LineWriter($"Started work at {DateTime.Now} on {file}");
        public void LineBreak()
            => LineWriter("--||--");
        public void TimeForFile(DateTime startTime)
            => LineWriter($"Time spent for file {DateTime.Now - startTime}");
        public void TimeTotal()
            => LineWriter($"Total time spent {DateTime.Now - StartTime}");
        public void Hash(string hash)
            => LineWriter($"MD5 Hash of file is: {hash} at {DateTime.Now}");
        public void ErrorTotal()
            => LineWriter($"Done! There were {ErrorCount} errors out of {ErrorCount + SuccessCount} files.");
        private void LineWriter(string lineToWrite)
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    File.WriteAllText(FilePath, lineToWrite);
                }
                else
                {
                    string toWrite = "\n" + lineToWrite;
                    File.AppendAllText(FilePath, toWrite);
                }
            }
            catch
            {
                MessageBox.Show("Проблемы с файлом логов");
            }
        }
        public void Dispose()
        {
            Path = default;
            StartTime = default;
            FileName = default;
            FilePath = default;
            ErrorCount = default;
            SuccessCount = default;
        }
    }
}