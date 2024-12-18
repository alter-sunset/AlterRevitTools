using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using AlterTools.BatchExportNet.Utils;
using AlterTools.BatchExportNet.Views.Base;
using AlterTools.BatchExportNet.Views.Transmit;

namespace AlterTools.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerTransmit : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not TransmitViewModel transmitVM || !transmitVM.IsEverythingFilled()) return;

            string folderPath = transmitVM.FolderPath;
            bool isSameFolder = transmitVM.IsSameFolder;

            foreach (ListBoxItem item in transmitVM.ListBoxItems)
            {
                string filePath = item.Content.ToString();

                if (!File.Exists(filePath))
                {
                    item.Background = Brushes.Red;
                    continue;
                }

                string transmittedFilePath = Path.Combine(folderPath, Path.GetFileName(filePath));
                File.Copy(filePath, transmittedFilePath, true);

                ModelPath transmittedModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(transmittedFilePath);
                transmittedModelPath.UnloadRevitLinks(folderPath, isSameFolder);
            }
            transmitVM.Finisher(id: "TransmitModelsFinished");
        }
    }
}