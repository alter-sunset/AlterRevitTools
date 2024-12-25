using AlterTools.DriveFromOutside.Events;

namespace AlterTools.DriveFromOutside.Utils
{
    public static class ConfigHelper
    {
        public static T GetEventConfig<T>(this TaskConfig taskConfig) => taskConfig.EventConfig.ToObject<T>();
    }
}