using System.Text.Json;
using VLS.DriveFromOutside.Events;
using VLS.DriveFromOutside.Events.Detach;
using VLS.DriveFromOutside.Events.IFC;
using VLS.DriveFromOutside.Events.NWC;
using VLS.DriveFromOutside.Events.Transmit;
using VLS.DriveFromOutside.Utils;

namespace VLS.DriveFromOutside
{
    public class ExternalTaskHandler(List<IEventHolder> eventHolders)
    {
        private readonly List<IEventHolder> _eventHolders = eventHolders;

        private static readonly string FOLDER_CONFIGS = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"RevitListener\Tasks");
        public async Task LookForSingleTask(TimeSpan period)
        {
            using PeriodicTimer timer = new(period);
            while (await timer.WaitForNextTickAsync())
            {
                TaskConfig taskConfig = GetOldestMessage();
                if (taskConfig != null)
                    RaiseEvent(taskConfig);
            }
        }
        public static TaskConfig GetOldestMessage()
        {
            string? file = Directory.GetFiles(FOLDER_CONFIGS)
                .OrderBy(File.GetLastWriteTime)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(file))
                return null;
            using FileStream fileStream = File.OpenRead(file);
            JsonDocument document = JsonDocument.Parse(fileStream);
            fileStream.Close();
            fileStream.Dispose();
            JsonElement root = document.RootElement;

            TaskConfig taskConfig = new()
            {
                ExternalEvent = root
                    .GetProperty("ExternalEvent")
                    .Deserialize<ExternalEvents>(),
                EventConfig = root.GetProperty("EventConfig"),
                FilePath = file
            };
            return taskConfig;
        }
        private void RaiseEvent(TaskConfig taskConfig)
        {
            IEventHolder eventHolder = _eventHolders.FirstOrDefault(e => e.ExternalEvent == taskConfig.ExternalEvent);
            if (eventHolder is null)
                return;
            switch (taskConfig.ExternalEvent)
            {
                case ExternalEvents.Transmit:
                    TransmitConfig transmitConfig = taskConfig.GetEventConfig<TransmitConfig>();
                    EventHandlerTransmit eventHandlerTransmit = eventHolder.ExternalEventHandler as EventHandlerTransmit;
                    eventHandlerTransmit.Raise(transmitConfig);
                    break;

                case ExternalEvents.Detach:
                    DetachConfig detachConfig = taskConfig.GetEventConfig<DetachConfig>();
                    EventHandlerDetach eventHandlerDetach = eventHolder.ExternalEventHandler as EventHandlerDetach;
                    eventHandlerDetach.Raise(detachConfig);
                    break;

                case ExternalEvents.NWC:
                    NWC_Config nwc_Config = taskConfig.GetEventConfig<NWC_Config>();
                    EventHandlerNWC eventHandlerNWC = eventHolder.ExternalEventHandler as EventHandlerNWC;
                    eventHandlerNWC.Raise(nwc_Config);
                    break;

                case ExternalEvents.IFC:
                    IFC_Config ifc_Config = taskConfig.GetEventConfig<IFC_Config>();
                    EventHandlerIFC eventHandlerIFC = eventHolder.ExternalEventHandler as EventHandlerIFC;
                    eventHandlerIFC.Raise(ifc_Config);
                    break;

                default:
                    return;
            }
            File.Delete(taskConfig.FilePath);
        }
    }
}