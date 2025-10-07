using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Transmit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerTransmit : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
    {
        if (iConfigBase is not TransmitViewModel transmitVm) return;

        if (!transmitVm.IsEverythingFilled()) return;

        string folderPath = transmitVm.FolderPath;

        foreach (ListBoxItem item in transmitVm.ListBoxItems)
        {
            item.Background = Brushes.Blue;
            string filePath = item.Content.ToString();

            if (!File.Exists(filePath))
            {
                item.Background = Brushes.Red;
                continue;
            }

            string transmittedFilePath = Path.Combine(folderPath, Path.GetFileName(filePath));

            File.Copy(filePath, transmittedFilePath, true);

            ModelPath transmittedModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(transmittedFilePath);
            transmittedModelPath.UnloadRevitLinks(folderPath, transmitVm.IsSameFolder);
            item.Background = Brushes.Green;
        }

        transmitVm.Finisher("TransmitModelsFinished");
    }
}