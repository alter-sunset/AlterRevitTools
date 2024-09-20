using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace VLS.BatchExportNet.Views
{
    public enum HelpMessages
    {
        Load,
        Folder,
        Naming,
        List,
        Config,
        Start,
        DetachTitle,
        DetachMid,
        IFCTitle,
        LinkTitle,
        Migrate,
        NWCTitle,
        NWCEnd,
        TransmitTitle
    }
    public static class Help
    {
        public static Dictionary<HelpMessages, string> GetHelpMessages()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream("VLS.BatchExportNet.Resources.HelpMessages.json");
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
            };
            return JsonSerializer.Deserialize<Dictionary<HelpMessages, string>>(s, options);
        }

        public static string GetResultMessage(this Dictionary<HelpMessages, string> helpDictionary,
            params HelpMessages[] helpCodes)
        {
            return string.Join('\n', helpCodes.Select(e => helpDictionary.GetValueOrDefault(e)));
        }
    }
}