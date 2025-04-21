using AlterTools.DriveFromOutside.Events;
using AlterTools.DriveFromOutside.Events.Detach;
using AlterTools.DriveFromOutside.Events.IFC;
using AlterTools.DriveFromOutside.Events.NWC;
using AlterTools.DriveFromOutside.Events.Transmit;
using AlterTools.DriveFromOutside.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AlterTools.DriveFromOutside;

public class ExternalTaskHandler(List<IEventHolder> eventHolders)
{
    private static readonly string FolderConfigs = InitializeFolderConfigs();

    private static string InitializeFolderConfigs()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"RevitListener\Tasks");
    }

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
                var taskConfig = GetOldestMessage();
                if (taskConfig is not null)
                    RaiseEvent(taskConfig);
            },
            null, TimeSpan.Zero, period);
    }
#endif

    private static TaskConfig GetOldestMessage()
    {
        var files = Directory.GetFiles(FolderConfigs)
            .OrderBy(File.GetLastWriteTime)
            .ToArray();

        if (files.Length == 0) return null!;

        using var fileStream = File.OpenText(files[0]);
        using JsonTextReader reader = new(fileStream);
        var jsonObject = JObject.Load(reader);

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
        var eventHolder = eventHolders.FirstOrDefault(e => e.ExternalEvent == taskConfig.ExternalEvent);
        if (eventHolder is null) return;

        var eventHandlers = GetEventHandlers(taskConfig, eventHolder);

        // Invoke the appropriate event handler if it exists
        if (eventHandlers.TryGetValue(taskConfig.ExternalEvent, out var raiseEvent)) raiseEvent();

        // Delete the file after raising the event
        File.Delete(taskConfig.FilePath);
    }

    private static Dictionary<ExternalEvents, Action> GetEventHandlers(TaskConfig taskConfig,
        IEventHolder eventHolder)
    {
        return new Dictionary<ExternalEvents, Action>
        {
            {
                ExternalEvents.Transmit, () =>
                {
                    var transmitConfig = taskConfig.GetEventConfig<TransmitConfig>();
                    var handler = eventHolder.ExternalEventHandler as EventHandlerTransmit;
                    handler?.Raise(transmitConfig);
                }
            },
            {
                ExternalEvents.Detach, () =>
                {
                    var detachConfig = taskConfig.GetEventConfig<DetachConfig>();
                    var handler = eventHolder.ExternalEventHandler as EventHandlerDetach;
                    handler?.Raise(detachConfig);
                }
            },
            {
                ExternalEvents.NWC, () =>
                {
                    var nwcConfig = taskConfig.GetEventConfig<NWCConfig>();
                    var handler = eventHolder.ExternalEventHandler as EventHandlerNWC;
                    handler?.Raise(nwcConfig);
                }
            },
            {
                ExternalEvents.IFC, () =>
                {
                    var ifcConfig = taskConfig.GetEventConfig<IFCConfig>();
                    var handler = eventHolder.ExternalEventHandler as EventHandlerIFC;
                    handler?.Raise(ifcConfig);
                }
            }
        };
    }
}