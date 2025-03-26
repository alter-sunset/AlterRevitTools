using System.Diagnostics;

namespace DriveFromOutsideServer.Revit
{
    public class RevitInstance : IDisposable
    {
        private const int _latestVersion = 2025;
        private readonly Process _revitProcess = new();
        private readonly SemaphoreSlim _semaphore;
        public int Version { get; private set; }
        public RevitInstance(int version, SemaphoreSlim semaphore)
        {
            if (version < 2019 || version > _latestVersion)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(version),
                    $"Revit version must be between 2019 and {_latestVersion}."
                );
            }

            _semaphore = semaphore;
            _semaphore.Wait();
            Version = version;
            _revitProcess.StartInfo.FileName = $"C:\\Program Files\\Autodesk\\Revit {Version}\\Revit.exe";
            _revitProcess.StartInfo.Arguments = $"D:\\test {Version}\\journal.txt"; //replace as something like AppData

            _revitProcess.Start();
        }

        public void Dispose()
        {
            _semaphore.Release();
            _revitProcess.Kill();
            _revitProcess.Dispose();
        }
    }
}