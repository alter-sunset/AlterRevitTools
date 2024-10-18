using SuperCopyMonitoring.ViewModels;

namespace SuperCopyMonitoring.Views
{
    public sealed partial class SuperCopyMonitoringView
    {
        public SuperCopyMonitoringView(SuperCopyMonitoringViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}