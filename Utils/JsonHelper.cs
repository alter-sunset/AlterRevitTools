using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace VLS.BatchExportNet.Utils
{
    public static class JsonHelper<T>
    {
        public static T DeserializeResource(string path) =>
            JsonSerializer.Deserialize<T>(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(path),
                GetDefaultOptions());

        public static T DeserializeConfig(FileStream file) =>
            JsonSerializer.Deserialize<T>(file, GetDefaultOptions());

        public static void SerializeConfig(T value, string path) =>
            File.WriteAllText(path, JsonSerializer.Serialize(value, GetDefaultOptions()));
        private static JsonSerializerOptions GetDefaultOptions() => new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };
    }
}