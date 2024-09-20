using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace VLS.BatchExportNet.Utils
{
    public static class JsonHelper
    {
        public static JsonSerializerOptions GetDefaultOptions()
        {
            return new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
            };
        }
    }
}
