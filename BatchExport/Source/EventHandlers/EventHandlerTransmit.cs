﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using VLS.BatchExport.Utils;
using VLS.BatchExport.Views.Base;
using VLS.BatchExport.Views.Transmit;

namespace VLS.BatchExport.Source.EventHandlers
{
    public class EventHandlerTransmit : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (!(iConfigBase is TransmitViewModel transmitVM) || !transmitVM.IsEverythingFilled()) return;

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