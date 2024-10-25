using BatchExportNet.Views.Base;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace VLS.BatchExportNet.Views.Base
{
    public class WindowBase : Window
    {
        protected override void OnClosing(CancelEventArgs e)
        {
            DataContext = null;
            base.OnClosing(e);
        }
        public void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                if (e.RemovedItems.Count != 0 && e.RemovedItems[0] is not IEntry) return;
                // Unselect previously selected entries
                foreach (IEntry entry in e.RemovedItems)
                {
                    entry.IsSelected = false; // Set IsSelected to false for unselected entries
                }

                if (e.AddedItems.Count != 0 && e.AddedItems[0] is not IEntry) return;
                // Select newly selected entries
                foreach (IEntry entry in e.AddedItems)
                {
                    entry.IsSelected = true; // Set IsSelected to true for newly selected entries
                }
            }
        }
    }
}