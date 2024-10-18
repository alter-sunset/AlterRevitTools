
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External.Handlers;
using SuperCopyMonitoring.Models;

namespace SuperCopyMonitoring.ViewModels
{
    public sealed partial class SuperCopyMonitoringViewModel : ObservableObject
    {
        private readonly AsyncEventHandler _copyMonitorEventHandler = new();

        private CopyMonitorHandler _copyMonitorHandler;

        [RelayCommand]
        private void CopySelected()
        {
            _copyMonitorEventHandler.RaiseAsync(uiApp =>
            {
                _copyMonitorHandler ??= new(uiApp);
                _copyMonitorHandler.CopySelected();
            });
        }
    }
}