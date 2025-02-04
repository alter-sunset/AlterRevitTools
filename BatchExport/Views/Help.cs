using System.Linq;
using AlterTools.BatchExport.Utils;
using Messages = System.Collections.Generic.Dictionary<AlterTools.BatchExport.Views.HelpMessageType, string>;

namespace AlterTools.BatchExport.Views
{
    public static class Help
    {
        public static Messages GetHelpDictionary()
            => JsonHelper<Messages>.DeserializeResource("AlterTools.BatchExport.Resources.HelpMessages.json");

        public static string GetResultMessage(this Messages helpDictionary, params HelpMessageType[] helpCodes)
            => string.Join("\n", helpCodes.Select(code
                => helpDictionary.TryGetValue(code, out var value) ? value : string.Empty));
    }
}