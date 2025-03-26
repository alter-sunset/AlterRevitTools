using AlterTools.DriveFromOutside.Events;
using Autodesk.Revit.UI;
using Microsoft.AspNetCore.SignalR.Client;

namespace AlterTools.DriveFromOutside
{
    public sealed class SignalRService
    {
        private static readonly Lazy<SignalRService> _instance = new(() => new SignalRService());

        private HubConnection _connection;

        private SignalRService() { }

        public static SignalRService Instance => _instance.Value;

        public async Task StartAsync(string hubUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()  // Built-in reconnection support
                .Build();

            // Configure message handler
            //add handler for each command
            _connection.On<string>("Transmit", TransmitCommand);

            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                // Handle connection errors
            }
        }

        private void TransmitCommand(string config)
        {
            RevitAppEvent.Raise(() => ProcessMessageInRevitContext(config));
        }

        private void HandleMessageReceived(string message)
        {
            // Use Revit API via ExternalEvent
            RevitAppEvent.Raise(() => ProcessMessageInRevitContext(message));
        }

        private void ProcessMessageInRevitContext(string message)
        {
            //process various commands
            // Safe to call Revit API here
            // (e.g., modify document, show alerts, etc.)
        }

        public async Task StopAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}