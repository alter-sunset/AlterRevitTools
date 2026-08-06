using System.Diagnostics;
using System.Windows.Interop;
using AlterTools.atExportModel.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.atExportModel;

[Transaction(TransactionMode.Manual)]
public class Command : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        ExternalEventHandler handler = new();
        ViewModelMain viewModel = new(handler);
        WindowMain window = new(viewModel);

        UIApplication uiApp = commandData.Application;
        // Link WPF window as a child of Revit
        WindowInteropHelper helper = new(window)
        {
            Owner = uiApp.MainWindowHandle
        };

        window.Show();

        return Result.Succeeded;
    }
}