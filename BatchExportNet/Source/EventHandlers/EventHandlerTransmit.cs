using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Transmit;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerTransmit : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase viewModelBase)
        {
            if (viewModelBase is not TransmitViewModel transmitViewModel || !transmitViewModel.IsEverythingFilled()) return;

            using Application application = uiApp.Application;
            string folderPath = transmitViewModel.FolderPath;
            bool isSameFolder = transmitViewModel.IsSameFolder;

            foreach (ListBoxItem item in transmitViewModel.ListBoxItems)
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
            transmitViewModel.Finisher(id: "TransmitModelsFinished");
        }
    }
}