using AlterTools.Utils;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JetBrains.Annotations;
using Application = Autodesk.Revit.ApplicationServices.Application;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace AlterTools.atMigrate;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class Command : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        using UIApplication uiApp = commandData.Application;
        using Application app = uiApp.Application;

        // Open csv catalogue with path pairs
        using OpenFileDialog openFileDialog = new();
        if (openFileDialog.ShowDialog() is not DialogResult.OK) return Result.Cancelled;
        string catalogue = openFileDialog.FileName;

        using (ErrorSuppressor _ = new(uiApp))
        {
            Helper.ProcessFiles(catalogue, app);
        }

        using TaskDialog taskDialog = new("Finished");
        taskDialog.CommonButtons = TaskDialogCommonButtons.Close;
        taskDialog.Id = "MigrateFinished";
        taskDialog.MainContent = "Models have been successfully migrated";

        taskDialog.Show();

        return Result.Succeeded;
    }
}