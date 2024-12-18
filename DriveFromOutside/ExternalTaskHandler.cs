using System.Text.Json;
using AlterTools.DriveFromOutside.Utils;
using AlterTools.DriveFromOutside.Events;
using AlterTools.DriveFromOutside.Events.IFC;
using AlterTools.DriveFromOutside.Events.NWC;
using AlterTools.DriveFromOutside.Events.Detach;
using AlterTools.DriveFromOutside.Events.Transmit;

namespace AlterTools.DriveFromOutside
{
    public class ExternalTaskHandler(List<IEventHolder> eventHolders)
    {
        private readonly List<IEventHolder> _eventHolders = eventHolders;

        private static readonly string FOLDER_CONFIGS = InitializeFolderConfigs();
        private static string InitializeFolderConfigs() =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"RevitListener\Tasks");

        public async Task LookForSingleTask(TimeSpan period)
        {
            using PeriodicTimer timer = new(period);
            while (await timer.WaitForNextTickAsync())
            {
                TaskConfig? taskConfig = GetOldestMessage();
                if (taskConfig is not null)
                    RaiseEvent(taskConfig);
            }
        }
        public static TaskConfig? GetOldestMessage()
        {
            string[] files = Directory.GetFiles(FOLDER_CONFIGS)
                .OrderBy(File.GetLastWriteTime)
                .ToArray();

            if (files.Length == 0) return null;

            using FileStream fileStream = File.OpenRead(files[0]);
            using JsonDocument jsonDoc = JsonDocument.Parse(fileStream);
            JsonElement root = jsonDoc.RootElement;

            return new TaskConfig
            {
                ExternalEvent = root.GetProperty("ExternalEvent").Deserialize<ExternalEvents>(),
                EventConfig = root.GetProperty("EventConfig"),
                FilePath = files[0]
            };
        }
        private void RaiseEvent(TaskConfig taskConfig)
        {
            if (taskConfig is null || taskConfig.FilePath is null) return;
            IEventHolder? eventHolder = _eventHolders
                .FirstOrDefault(e => e.ExternalEvent == taskConfig.ExternalEvent);
            if (eventHolder is null) return;

            Dictionary<ExternalEvents, Action> eventHandlers = GetEventHandlers(taskConfig, eventHolder);

            // Invoke the appropriate event handler if it exists
            if (eventHandlers.TryGetValue(taskConfig.ExternalEvent, out var raiseEvent))
                raiseEvent();

            // Delete the file after raising the event
            File.Delete(taskConfig.FilePath);
        }

        private static Dictionary<ExternalEvents, Action> GetEventHandlers(TaskConfig taskConfig,
            IEventHolder eventHolder)
        {
            return new()
            {
                { ExternalEvents.Transmit, () =>
                    {
                        TransmitConfig transmitConfig = taskConfig.GetEventConfig<TransmitConfig>();
                        EventHandlerTransmit? handler = eventHolder.ExternalEventHandler as EventHandlerTransmit;
                        handler?.Raise(transmitConfig);
                    }
                },
                { ExternalEvents.Detach, () =>
                    {
                        DetachConfig detachConfig = taskConfig.GetEventConfig<DetachConfig>();
                        EventHandlerDetach? handler = eventHolder.ExternalEventHandler as EventHandlerDetach;
                        handler?.Raise(detachConfig);
                    }
                },
                { ExternalEvents.NWC, () =>
                    {
                        NwcConfig nwcConfig = taskConfig.GetEventConfig<NwcConfig>();
                        EventHandlerNWC? handler = eventHolder.ExternalEventHandler as EventHandlerNWC;
                        handler?.Raise(nwcConfig);
                    }
                },
                { ExternalEvents.IFC, () =>
                    {
                        IfcConfig ifcConfig = taskConfig.GetEventConfig<IfcConfig>();
                        EventHandlerIFC? handler = eventHolder.ExternalEventHandler as EventHandlerIFC;
                        handler?.Raise(ifcConfig);
                    }
                },
            };
        }
    }
}