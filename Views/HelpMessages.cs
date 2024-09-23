using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using VLS.BatchExportNet.Utils;
using Messages = System.Collections.Generic.Dictionary<VLS.BatchExportNet.Views.HelpMessages, string>;

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
        public static Messages GetHelpMessages()
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