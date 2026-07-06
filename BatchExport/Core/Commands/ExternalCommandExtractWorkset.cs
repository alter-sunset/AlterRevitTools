using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views;
using Autodesk.Revit.Attributes;
using Application = Autodesk.Revit.ApplicationServices.Application;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.ReadOnly)]
public class ExternalCommandExtractWorkset : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        using UIApplication uiApp = commandData.Application;
        using Application app = uiApp.Application;
        using FolderBrowserDialog folderDialog = new();
        if (folderDialog.ShowDialog() is not DialogResult.OK)
        {
            return Result.Cancelled;
        }

        using SaveFileDialog saveFileDialog = DialogType.SingleCsv.SaveFileDialog();
        if (saveFileDialog.ShowDialog() is not DialogResult.OK)
        {
            return Result.Cancelled;
        }

        ErrorSuppressor errorSuppressor = new(uiApp);

        string[] files = Directory.GetFiles(folderDialog.SelectedPath, "*.rvt", SearchOption.AllDirectories);

        using CsvHelper csvHelper = new(saveFileDialog.FileName, ["ModelName", "WorksetName"]);

        foreach (string file in files)
        {
            string modelName = Path.GetFileName(file);

            BasicFileInfo info = BasicFileInfo.Extract(file);

            if (!info.IsWorkshared)
            {
                // No user worksets to extract.
                continue;
            }

            if (info.IsSavedInLaterVersion)
            {
                csvHelper.WriteWorkset(Path.GetFileName(file), "ERROR: Saved in later Revit version");
                continue;
            }

            try
            {
                IEnumerable<string> worksets = GetWorksetNames(app, file);

                foreach (string workset in worksets)
                {
                    csvHelper.WriteWorkset(modelName, workset);
                }
            }
            catch (Exception ex)
            {
                // Optional: write failure info to CSV/log instead of stopping the batch.
                csvHelper.WriteWorkset(modelName, $"ERROR: {ex.Message}");
            }
        }

        errorSuppressor.Dispose();

        using TaskDialog taskDialog = new("Закончил");
        taskDialog.CommonButtons = TaskDialogCommonButtons.Close;
        taskDialog.Id = "WorksetExtractFinish";
        taskDialog.MainContent = "Закончил.";

        taskDialog.Show();

        return Result.Succeeded;
    }

    private static IEnumerable<string> GetWorksetNames(Application app, string file)
    {
        using ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(file);

        try
        {
            // Fast path: does not open the model.
            return WorksharingUtils
                .GetUserWorksetInfo(modelPath)
                .Select(x => x.Name)
                .ToList();
        }
        catch
        {
            // Fallback: open detached, read worksets, close without saving.
            // This avoids resolving the original central model.
        }

        Document doc = null;

        try
        {
            OpenOptions openOptions = new()
            {
                DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets
            };

            // Optional: reduce load time.
            WorksetConfiguration worksetConfig = new(WorksetConfigurationOption.CloseAllWorksets);

            openOptions.SetOpenWorksetsConfiguration(worksetConfig);

            doc = app.OpenDocumentFile(modelPath, openOptions);

            return new FilteredWorksetCollector(doc)
                .OfKind(WorksetKind.UserWorkset)
                .Select(x => x.Name)
                .ToList();
        }
        finally
        {
            doc?.Close(false);
            string tempDir = file.Replace(".rvt", "_backup");
            Directory.Delete(tempDir, true);
        }
    }
}