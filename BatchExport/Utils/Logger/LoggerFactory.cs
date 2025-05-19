namespace AlterTools.BatchExport.Utils.Logger;

public static class LoggerFactory
{
    public static ILogger CreateLogger(string path, bool offed)
    {
        return offed ? new NullLogger() : new FileLogger(path);
    }
}