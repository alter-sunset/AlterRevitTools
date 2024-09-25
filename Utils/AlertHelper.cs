using Alerts = System.Collections.Generic.Dictionary<VLS.BatchExportNet.Utils.AlertType, string>;

namespace VLS.BatchExportNet.Utils
{
    public static class AlertHelper
    {
        public static Alerts GetAlerts() =>
            JsonHelper<Alerts>.DeserializeResource("VLS.BatchExportNet.Resources.AlertMessages.json");
    }
}
