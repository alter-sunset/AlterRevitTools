using System.Diagnostics;

namespace DriveFromOutsideServer.Revit
{
    public class RevitInstance : IDisposable
    {
        private const int _latestVersion = 2025;
        private readonly Process _revitProcess = new();
        public int Version { get; private set; }
        public RevitInstance(int version)
        {
            if (version < 2019 || version > _latestVersion)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(version),
                    $"Revit version must be between 2019 and {_latestVersion}."
                );
            }

            Version = version;
            _revitProcess.StartInfo.FileName = $"C:\\Program Files\\Autodesk\\Revit {Version}\\Revit.exe";
            _revitProcess.StartInfo.Arguments = $"D:\\test {Version}\\journal.txt"; //replace as something like AppData

            _revitProcess.Start();
        }

        public void Dispose()
        {
            _revitProcess.Kill();
            _revitProcess.Dispose();
        }
    }
}