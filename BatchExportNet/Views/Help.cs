using System.Linq;
using System.Collections.Generic;
using VLS.BatchExportNet.Utils;
using Messages = System.Collections.Generic.Dictionary<VLS.BatchExportNet.Views.HelpMessageType, string>;

namespace VLS.BatchExportNet.Views
{
    public static class Help
    {
        public static Messages GetHelpDictionary() =>
            JsonHelper<Messages>.DeserializeResource("VLS.BatchExportNet.Resources.HelpMessages.json");

        public static string GetResultMessage(this Messages helpDictionary,
                params HelpMessageType[] helpCodes) =>
            string.Join('\n', helpCodes.Select(e => helpDictionary.GetValueOrDefault(e)));
    }
}