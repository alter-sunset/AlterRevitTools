using System;
using System.Linq;
using AlterTools.BatchExport.Utils;
using Messages = System.Collections.Generic.Dictionary<AlterTools.BatchExport.Views.HelpMessageType, string>;

namespace AlterTools.BatchExport.Views;

public static class Help
{
    private const string HelpFile = "AlterTools.BatchExport.Resources.HelpMessages.json";

    public static Messages GetHelpDictionary()
    {
        return JsonHelper<Messages>.DeserializeResource(HelpFile);
    }

    public static string GetResultMessage(this Messages helpDictionary, params HelpMessageType[] helpCodes)
    {
        return string.Join(Environment.NewLine,
            helpCodes.Select(code => helpDictionary.TryGetValue(code, out string value) ? value : string.Empty));
    }
}