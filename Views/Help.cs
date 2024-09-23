using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Collections.Generic;
using VLS.BatchExportNet.Utils;
using Messages = System.Collections.Generic.Dictionary<VLS.BatchExportNet.Views.HelpMessages, string>;

namespace VLS.BatchExportNet.Views
{
    public static class Help
    {
        public static Messages GetHelpDictionary()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Stream s = a.GetManifestResourceStream("VLS.BatchExportNet.Resources.HelpMessages.json");
            JsonSerializerOptions options = JsonHelper.GetDefaultOptions();
            return JsonSerializer.Deserialize<Messages>(s, options);
        }

        public static string GetResultMessage(this Messages helpDictionary,
            params HelpMessages[] helpCodes)
        {
            return string.Join('\n', helpCodes.Select(e => helpDictionary.GetValueOrDefault(e)));
        }
    }
}