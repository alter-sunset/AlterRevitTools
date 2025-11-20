using System.Windows.Controls;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Detach;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Brushes = System.Windows.Media.Brushes;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerDetach : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
    {
        if (iConfigBase is not DetachViewModel detachVm) return;
        if (!detachVm.IsEverythingFilled()) return;

        using Application app = uiApp.Application;
        ErrorSuppressor errorSuppressor = new(uiApp);

        List<ListBoxItem> listItems = [.. detachVm.ListBoxItems];

        foreach (ListBoxItem item in listItems)
        {
            item.Background = Brushes.Blue;
            string filePath = item.Content?.ToString();

            if (!File.Exists(filePath))
            {
                item.Background = Brushes.Red;
                continue;
            }

            detachVm.DetachModel(app, filePath);
            item.Background = Brushes.Green;
        }
        errorSuppressor.Dispose();

        detachVm.FinishWork("DetachModelsFinished");
    }
}