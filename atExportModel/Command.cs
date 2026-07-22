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
        MainViewModel viewModel = new(handler);
        MainWindow window = new(viewModel);
        window.Show();

        return Result.Succeeded;
    }
}