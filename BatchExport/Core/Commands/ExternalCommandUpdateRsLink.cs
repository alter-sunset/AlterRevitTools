using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Utils.Extensions;
using Autodesk.Revit.Attributes;
using Application = Autodesk.Revit.ApplicationServices.Application;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandUpdateRsLink : IExternalCommand
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

        using OpenFileDialog openFileDialog = new();
        if (openFileDialog.ShowDialog() is not DialogResult.OK) return Result.Cancelled;
        string catalogue = openFileDialog.FileName;

        Dictionary<string, string> pathPairs = File.ReadLines(catalogue)
            .Select(l => l.Split('|'))
            .ToDictionary(l => l[0].Trim(), l => l[1].Trim());

        foreach ((string tempPath, string newPath) in pathPairs)
        {
            if (!File.Exists(tempPath)) continue;
            ModelPath tempMPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(tempPath);
            using Document doc =
                tempMPath.OpenDetached(app, new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets));

            using ErrorSuppressor errorSuppressor = new(uiApp);

            doc.DeleteAllLinks(false);
            doc.PurgeAll();

            ModelPath newMPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(newPath);
            doc.SaveAs(newMPath, saveAsOptions);
        }

        using TaskDialog taskDialog = new("Finished");
        taskDialog.CommonButtons = TaskDialogCommonButtons.Close;
        taskDialog.Id = "UpdateRsLinkFinished";
        taskDialog.MainContent = "Models have been successfully updated";

        taskDialog.Show();

        return Result.Succeeded;
    }
}