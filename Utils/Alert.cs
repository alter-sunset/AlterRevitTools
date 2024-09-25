using System.Collections.Generic;
using Alerts = System.Collections.Generic.Dictionary<VLS.BatchExportNet.Utils.AlertType, string>;

namespace VLS.BatchExportNet.Utils
{
    public static class Alert
    {
        public static string GetAlert(this AlertType alertType) =>
            JsonHelper<Alerts>
                .DeserializeResource("VLS.BatchExportNet.Resources.AlertMessages.json")
                .GetValueOrDefault(alertType);
    }
}
