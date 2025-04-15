using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AlterTools.BatchExport.Core.Commands
{
    public static class ExtractWorksetsCommand
    {
        public static Result Execute()
        {
            FolderBrowserDialog folderDialog = new();
            if (folderDialog.ShowDialog() is not DialogResult.OK) return Result.Cancelled;

            SaveFileDialog saveFileDialog = DialogType.SingleCsv.SaveFileDialog();
            if (saveFileDialog.ShowDialog() is not DialogResult.OK) return Result.Cancelled;

            string resultingFile = saveFileDialog.FileName;
            string[] files = Directory.GetFiles(folderDialog.SelectedPath, "*.rvt", SearchOption.AllDirectories);

            using CsvHelper csvHelper = new(resultingFile);
            foreach (string file in files)
            {
                string modelName = Path.GetFileName(file);

                IEnumerable<string> worksets = WorksharingUtils.GetUserWorksetInfo(ModelPathUtils.ConvertUserVisiblePathToModelPath(file))
                    .Select(w => w.Name);

                foreach (string workset in worksets)
                {
                    csvHelper.WriteWorkset(modelName, workset);
                }
            }

            return Result.Succeeded;
        }
    }
}