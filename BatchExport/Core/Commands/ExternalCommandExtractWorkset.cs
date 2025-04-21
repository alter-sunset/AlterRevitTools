using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JetBrains.Annotations;

namespace AlterTools.BatchExport.Core.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.ReadOnly)]
    public class ExternalCommandExtractWorkset : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            FolderBrowserDialog folderDialog = new();
            if (DialogResult.OK != folderDialog.ShowDialog())
            {
                return Result.Cancelled;
            }

            SaveFileDialog saveFileDialog = DialogType.SingleCsv.SaveFileDialog();
            if (DialogResult.OK != saveFileDialog.ShowDialog())
            {
                return Result.Cancelled;
            }

            string resultingFile = saveFileDialog.FileName;
            string[] files = Directory.GetFiles(folderDialog.SelectedPath, "*.rvt", SearchOption.AllDirectories);

            using CsvHelper csvHelper = new(resultingFile);

            foreach (string file in files)
            {
                string modelName = Path.GetFileName(file);

                IEnumerable<string> worksets = WorksharingUtils
                    .GetUserWorksetInfo(ModelPathUtils.ConvertUserVisiblePathToModelPath(file))
                    .Select(workset => workset.Name);

                foreach (string workset in worksets)
                {
                    csvHelper.WriteWorkset(modelName, workset);
                }
            }

            return Result.Succeeded;
        }
    }
}