using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.atExportModel;

[Transaction(TransactionMode.Manual)]
public class Command : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIApplication uiApp = commandData.Application;
        Document doc = uiApp.ActiveUIDocument.Document;

        // Initialize Revit thread marshaling loops
        RevitAddinEventHandler handler = new RevitAddinEventHandler();
        ExternalEvent externalEvent = ExternalEvent.Create(handler);

        // Spin up the Core MVVM Layers
        MainViewModel viewModel = new MainViewModel(externalEvent, handler);
        viewModel.LoadData(doc); // Initialize model parameters smoothly before UI load

        MainWindow window = new MainWindow(viewModel);

        // Modal window blocks Revit user thread safely until window closure
        // Use window.Show() for modeless UI windows alongside Revit operations
        window.ShowDialog();

        return Result.Succeeded;
    }
}