using System.Windows.Controls;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Utils.Extensions;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Transmit;
using Brushes = System.Windows.Media.Brushes;

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

        transmitVm.FinishWork("TransmitModelsFinished");
    }
}