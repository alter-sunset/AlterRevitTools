using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using SuperCopyMonitoring.ViewModels;
using SuperCopyMonitoring.Views;

namespace SuperCopyMonitoring.Commands
{
    /// <summary>
    ///     External command entry point invoked from the Revit interface
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class StartupCommand : ExternalCommand
    {
        public override void Execute()
        {
            SuperCopyMonitoringViewModel viewModel = new();
            SuperCopyMonitoringView view = new(viewModel);
            view.Show(UiApplication.MainWindowHandle);
        }
    }
}