using Autodesk.Revit.UI;

namespace AlterTools.DriveFromOutside
{
    public class RevitAppEvent : IExternalEventHandler
    {
        private static readonly RevitAppEvent _instance = new RevitAppEvent();
        private static ExternalEvent _externalEvent;
        private Action _action;

        public static void Initialize(UIApplication uiApp)
        {
            _externalEvent = ExternalEvent.Create(_instance);
        }

        public static void Raise(Action action)
        {
            _instance._action = action;
            _externalEvent.Raise();
        }

        public void Execute(UIApplication app)
        {
            _action?.Invoke();
            _action = null;
        }

        public string GetName() => "SignalR Revit Event";
    }
}
