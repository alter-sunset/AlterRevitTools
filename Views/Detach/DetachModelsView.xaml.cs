using System.ComponentModel;
using System.Windows;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Detach
{
    /// <summary>
    /// Interaction logic for DetachModelsView.xaml
    /// </summary>
    public partial class DetachModelsView : Window
    {
        public DetachModelsView(EventHandlerDetachModelsVMArg eventHandlerDetachModelsVMArg)
        {
            InitializeComponent();
            DataContext = new DetachViewModel(eventHandlerDetachModelsVMArg);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            DataContext = null;
            base.OnClosing(e);
        }
    }
}