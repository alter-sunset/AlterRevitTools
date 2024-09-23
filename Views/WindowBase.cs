using System.ComponentModel;
using System.Windows;

namespace VLS.BatchExportNet.Views
{
    public class WindowBase : Window
    {
        protected override void OnClosing(CancelEventArgs e)
        {
            DataContext = null;
            base.OnClosing(e);
        }
    }
}