using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private static readonly string FolderConfigs = InitializeFolderConfigs();
        private static string InitializeFolderConfigs() =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"RevitListener\Tasks");

#if R25_OR_GREATER
        public async Task LookForSingleTask(TimeSpan period)
        {
            using PeriodicTimer timer = new(period);
            while (await timer.WaitForNextTickAsync())
            {
                TaskConfig? taskConfig = GetOldestMessage();
                RaiseEvent(taskConfig);
            }
        }
#else
        public void LookForSingleTask(TimeSpan period)
        {
            Timer timer = null;

            timer = new Timer(_ =>
            {
                TaskConfig taskConfig = GetOldestMessage();
                if (taskConfig is not null)
                    RaiseEvent(taskConfig);
            },
            null, TimeSpan.Zero, period);
        }
#endif

        private static TaskConfig GetOldestMessage()
        {
            string[] files = Directory.GetFiles(FolderConfigs)
                .OrderBy(File.GetLastWriteTime)
                .ToArray();

            if (files.Length == 0) return null!;

            using StreamReader fileStream = File.OpenText(files[0]);
            using JsonTextReader reader = new(fileStream);
            JObject jsonObject = JObject.Load(reader);

            return new TaskConfig
            {
                ExternalEvent = jsonObject["ExternalEvent"]!.ToObject<ExternalEvents>(),
                EventConfig = jsonObject["EventConfig"],
                FilePath = files[0]
            };
        }

        private void RaiseEvent(TaskConfig taskConfig)
        {
            if (null == taskConfig?.FilePath) return;
            IEventHolder? eventHolder = eventHolders.FirstOrDefault(e => e.ExternalEvent == taskConfig.ExternalEvent);
            if (eventHolder is null) return;

            Dictionary<ExternalEvents, Action> eventHandlers = GetEventHandlers(taskConfig, eventHolder);

            // Invoke the appropriate event handler if it exists
            if (eventHandlers.TryGetValue(taskConfig.ExternalEvent, out var raiseEvent))
            {
                raiseEvent();
            }

            // Delete the file after raising the event
            File.Delete(taskConfig.FilePath);
        }

        private static Dictionary<ExternalEvents, Action> GetEventHandlers(TaskConfig taskConfig,
                                                                           IEventHolder eventHolder)
        {
            return new Dictionary<ExternalEvents, Action>
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
                        NWCConfig nwcConfig = taskConfig.GetEventConfig<NWCConfig>();
                        EventHandlerNWC? handler = eventHolder.ExternalEventHandler as EventHandlerNWC;
                        handler?.Raise(nwcConfig);
                    }
                },
                { ExternalEvents.IFC, () =>
                    {
                        IFCConfig ifcConfig = taskConfig.GetEventConfig<IFCConfig>();
                        EventHandlerIFC? handler = eventHolder.ExternalEventHandler as EventHandlerIFC;
                        handler?.Raise(ifcConfig);
                    }
                },
            };
        }
    }
}