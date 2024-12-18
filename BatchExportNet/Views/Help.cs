using System.Linq;
using System.Collections.Generic;
using AlterTools.BatchExportNet.Utils;
using Messages = System.Collections.Generic.Dictionary<AlterTools.BatchExportNet.Views.HelpMessageType, string>;

namespace AlterTools.BatchExportNet.Views
{
    public static class Help
    {
        public static Messages GetHelpDictionary() =>
            JsonHelper<Messages>.DeserializeResource("BatchExportNet.Resources.HelpMessages.json");

        public static string GetResultMessage(this Messages helpDictionary,
                params HelpMessageType[] helpCodes) =>
            string.Join('\n', helpCodes.Select(e => helpDictionary.GetValueOrDefault(e)));
    }
}