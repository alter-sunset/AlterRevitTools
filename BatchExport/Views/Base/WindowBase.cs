using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace VLS.BatchExport.Views.Base
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
            if (!(sender is ListBox)) return;
            if (e.RemovedItems.Count != 0 && !(e.RemovedItems[0] is ISelectable)) return;
            // Unselect previously selected entries            
            foreach (ISelectable entry in e.RemovedItems)
            {
                entry.IsSelected = false; // Set IsSelected to false for unselected entries
            }

            if (e.AddedItems.Count != 0 && !(e.AddedItems[0] is ISelectable)) return;
            // Select newly selected entries
            foreach (ISelectable entry in e.AddedItems)
            {
                entry.IsSelected = true; // Set IsSelected to true for newly selected entries
            }
        }
    }
}