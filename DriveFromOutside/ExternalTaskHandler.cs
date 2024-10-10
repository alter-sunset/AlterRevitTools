using Autodesk.Revit.UI;
using System.Text.Json;
using VLS.BatchExportNet.Source.EventHandlers;
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

        private static readonly string FOLDER_CONFIGS = InitializeFolderConfigs();
        private static string InitializeFolderConfigs()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"RevitListener\Tasks");
        }
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
            string[] files = [.. Directory.GetFiles(FOLDER_CONFIGS).OrderBy(File.GetLastWriteTime)];

            if (files.Length == 0)
                return null;

            using FileStream fileStream = File.OpenRead(files[0]);
            using JsonDocument document = JsonDocument.Parse(fileStream);
            JsonElement root = document.RootElement;

            return new TaskConfig
            {
                ExternalEvent = root.GetProperty("ExternalEvent").Deserialize<ExternalEvents>(),
                EventConfig = root.GetProperty("EventConfig"),
                FilePath = files[0]
            };
        }
        private void RaiseEvent(TaskConfig taskConfig)
        {
            IEventHolder eventHolder = _eventHolders
                .FirstOrDefault(e => e.ExternalEvent == taskConfig.ExternalEvent);
            if (eventHolder is null)
                return;

            // Create a dictionary mapping external events to their handlers
            Dictionary<ExternalEvents, Action> eventHandlers = new()
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
                        NWC_Config nwcConfig = taskConfig.GetEventConfig<NWC_Config>();
                        EventHandlerNWC? handler = eventHolder.ExternalEventHandler as EventHandlerNWC;
                        handler?.Raise(nwcConfig);
                    }
                },
                { ExternalEvents.IFC, () =>
                    {
                        IFC_Config ifcConfig = taskConfig.GetEventConfig<IFC_Config>();
                        EventHandlerIFC? handler = eventHolder.ExternalEventHandler as EventHandlerIFC;
                        handler?.Raise(ifcConfig);
                    }
                },
            };

            // Invoke the appropriate event handler if it exists
            if (eventHandlers.TryGetValue(taskConfig.ExternalEvent, out var raiseEvent))
            {
                raiseEvent();
            }

            // Delete the file after raising the event
            File.Delete(taskConfig.FilePath);
        }
    }
}