using System;

namespace AlterTools.BatchExport.Utils.Logger;

public class NullLogger : ILogger
{
    public int ErrorCount => 0;
    public void Error(string error, Exception ex = null) { }
    public void Info(string info) { }
    public void Start(string file) { }
    public void FileOpened() { }
    public void Success(string message) { }
    public void LineBreak() { }
    public void Hash(string hash) { }
    public void TimeForFile(DateTime startTime) { }
    public void TimeTotal() { }
    public void ErrorTotal() { }
    public void Dispose() { }
}