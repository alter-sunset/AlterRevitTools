using System.Collections.Generic;

namespace VLS.BatchExportNet.Utils
{
    public static class Alert
    {
        public static string GetAlert(this AlertType alertType) =>
            JsonHelper<Dictionary<AlertType, string>>
                .DeserializeResource("VLS.BatchExportNet.Resources.AlertMessages.json")
                .GetValueOrDefault(alertType);
    }
}