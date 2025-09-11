using System;

namespace AlterTools.BatchExport.Utils.Logger;

public interface ILogger : IDisposable
{
    int ErrorCount { get; }
    void Error(string error, Exception ex = null);
    void Info(string info);
    void Start(string file);
    void FileOpened();
    void Success(string message);
    void LineBreak();
    void Hash(string hash);
    void TimeForFile(DateTime startTime);
    void TimeTotal();
    void ErrorTotal();
}