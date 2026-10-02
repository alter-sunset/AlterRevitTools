using AlterTools.Resources;
using AlterTools.Utils;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.atMigrate;

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

        MessageBox.Show(Strings.Done);

        return Result.Succeeded;
    }
}