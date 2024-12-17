using System.Linq;
using VLS.BatchExport.Utils;
using Messages = System.Collections.Generic.Dictionary<VLS.BatchExport.Views.HelpMessageType, string>;

namespace VLS.BatchExport.Views
{
    public static class Help
    {
        public static Messages GetHelpDictionary()
            => JsonHelper<Messages>.DeserializeResource("BatchExport.Resources.HelpMessages.json");

        public static string GetResultMessage(this Messages helpDictionary, params HelpMessageType[] helpCodes)
            => string.Join("\n", helpCodes.Select(code
                => helpDictionary.TryGetValue(code, out var value) ? value : string.Empty));
    }
}