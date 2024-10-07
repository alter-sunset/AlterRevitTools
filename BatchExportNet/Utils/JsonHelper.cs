using System;
using System.IO;
using System.Windows;
using System.Reflection;
using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;

namespace VLS.BatchExportNet.Utils
{
    public static class JsonHelper<T>
    {
        public static T DeserializeResource(string path) =>
            JsonSerializer.Deserialize<T>(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(path),
                GetDefaultOptions());

        public static T DeserializeConfig(FileStream file)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(file, GetDefaultOptions());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неверная схема файла\n{ex.Message}");
                return default;
            }
        }

        public static void SerializeConfig(T value, string path)
        {
            try
            {
                File.WriteAllText(path, JsonSerializer.Serialize(value, GetDefaultOptions()));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неверная схема файла\n{ex.Message}");
            }
        }
        private static JsonSerializerOptions GetDefaultOptions() => new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };
    }
}