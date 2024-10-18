using Nice3point.Revit.Toolkit.External;
using SuperCopyMonitoring.Commands;
using Autodesk.Revit.UI;

namespace SuperCopyMonitoring
{
    /// <summary>
    ///     Application entry point
    /// </summary>
    [UsedImplicitly]
    public class Application : ExternalApplication
    {
        public override void OnStartup()
        {
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            RibbonPanel panel = Application.CreatePanel("Commands", "SuperCopyMonitoring");

            panel.AddPushButton<StartupCommand>("Execute")
                .SetImage("/SuperCopyMonitoring;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/SuperCopyMonitoring;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}