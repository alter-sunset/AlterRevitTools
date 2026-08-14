using System.IO;
using AlterTools.Resources;
using AlterTools.Utils;
using AlterTools.Utils.Extensions;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.atUpdateRsLink;

[Transaction(TransactionMode.Manual)]
public class Command : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        using UIApplication uiApp = commandData.Application;
        using Application app = uiApp.Application;

        using WorksharingSaveAsOptions worksharingSaveAsOptions = new();
        worksharingSaveAsOptions.SaveAsCentral = true;

        using SaveAsOptions saveAsOptions = new();
        saveAsOptions.OverwriteExistingFile = true;
        saveAsOptions.SetWorksharingOptions(worksharingSaveAsOptions);

        // Open csv catalogue with path pairs
        using OpenFileDialog openFileDialog = new();
        if (openFileDialog.ShowDialog() is not DialogResult.OK) return Result.Cancelled;
        string catalogue = openFileDialog.FileName;

        Dictionary<string, string> pathPairs = File.ReadLines(catalogue)
            .Select(l => l.Split('|'))
            .ToDictionary(l => l[0].Trim(), l => l[1].Trim());

        ErrorSuppressor errorSuppressor = new(uiApp);

        foreach (KeyValuePair<string, string> pair in pathPairs)
        {
            if (!File.Exists(pair.Key)) continue;
            ModelPath tempMPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(pair.Key);
            using Document doc =
                tempMPath.OpenDetached(app, new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets));

            doc.DeleteAllLinks(false); // Remove all linked documents
            doc.PurgeAll(); // Remove all unused elements

            ModelPath newMPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(pair.Value);
            doc.SaveAs(newMPath, saveAsOptions);
        }

        errorSuppressor.Dispose();

        MessageBox.Show(Strings.Done);

        return Result.Succeeded;
    }
}