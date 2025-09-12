using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AlterTools.BatchExport.Views.Base;

public class WindowBase : Window
{
    protected override void OnClosing(CancelEventArgs args)
    {
        DataContext = null;
        base.OnClosing(args);
    }

    protected void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        if (sender is not ListBox) return;

        if (args.RemovedItems.Count != 0 && args.RemovedItems[0] is not ISelectable) return;

        // Unselect previously selected entries            
        foreach (ISelectable entry in args.RemovedItems)
        {
            entry.IsSelected = false; // Set IsSelected to false for unselected entries
        }

        if (args.AddedItems.Count != 0 && args.AddedItems[0] is not ISelectable) return;

        // Select newly selected entries
        foreach (ISelectable entry in args.AddedItems)
        {
            entry.IsSelected = true; // Set IsSelected to true for newly selected entries
        }
    }
}