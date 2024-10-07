using System.Text.Json;
using Autodesk.Revit.ApplicationServices;
using VLS.DriveFromOutside.Events;
using VLS.DriveFromOutside.Events.Detach;
using VLS.DriveFromOutside.Events.NWC;
using VLS.DriveFromOutside.Events.Transmit;
using VLS.DriveFromOutside.Utils;

namespace VLS.DriveFromOutside
{
    public class ExternalTaskHandler(Application application, List<IEventHolder> eventHolders) : IExternalTaskHandler
    {
        private readonly Application _application = application;
        private readonly List<IEventHolder> _eventHolders = eventHolders;

        private static readonly string FOLDER_CONFIGS = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"RevitListener\Tasks");

        /// <summary>
        /// Method that will listen for new Tasks and run them accordingly
        /// </summary>
        /// <param name="period">Frequency of method execution</param>
        /// <returns></returns>
        public async Task ListenForNewTasks(TimeSpan period)
        {
            using PeriodicTimer timer = new(period);
            while (await timer.WaitForNextTickAsync())
            {
                List<TaskConfig> configs = ReadMessages();

                foreach (TaskConfig config in configs)
                {
                    RaiseEvent(config);
                }
            }
        }
        /// <summary>
        /// Method that will obtain messages
        /// </summary>
        public List<TaskConfig> ReadMessages()
        {
            List<TaskConfig> configs = [];
            string[] files = Directory.GetFiles(FOLDER_CONFIGS);
            foreach (string file in files)
            {
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

                    EventConfig = root.GetProperty("EventConfig")
                };
                configs.Add(taskConfig);
                File.Delete(file);
            }
            return configs;
        }

        private void RaiseEvent(TaskConfig taskConfig)
        {
            IEventHolder? eventHolder = _eventHolders.FirstOrDefault(e => e.ExternalEvent == taskConfig.ExternalEvent);
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

                default:
                    return;
            }
        }
    }
}