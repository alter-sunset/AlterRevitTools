using System.Text.Json;
using VLS.DriveFromOutside.Events;

namespace VLS.DriveFromOutside.Utils
{
    public static class ConfigHelper
    {
        public static T GetEventConfig<T>(this TaskConfig taskConfig)
            => ((JsonElement)taskConfig.EventConfig).Deserialize<T>();
    }
}