using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.atParams;

[Transaction(TransactionMode.Manual)]
public class Command : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIApplication uiApp = commandData.Application;

        ExternalEventHandler handler = new();
        ViewModelMain viewModel = new(handler);
        WindowMain window = new(viewModel);

        // Link WPF window as a child of Revit
        WindowInteropHelper helper = new(window)
        {
            Owner = uiApp.MainWindowHandle
        };

        window.Show();

        return Result.Succeeded;
    }
}