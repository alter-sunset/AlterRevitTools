﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Transmit;
using VLS.BatchExportNet.Views;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerTransmitModelsVMArg : EventHandlerBaseVMArgs
    {
        public override void Execute(UIApplication uiApp, ViewModelBase viewModelBase)
        {
            TransmitViewModel transmitViewModel = viewModelBase as TransmitViewModel;
            if (!transmitViewModel.IsEverythingFilled())
            {
                return;
            }

            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = [.. transmitViewModel.ListBoxItems];

            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();

                if (!File.Exists(filePath))
                {
                    string error = $"Файла {filePath} не существует. Ты совсем Туттуру?";
                    item.Background = Brushes.Red;
                    continue;
                }

                string folder = transmitViewModel.FolderPath;
                bool isSameFolder = transmitViewModel.IsSameFolder;

                string transmittedFilePath = folder + "\\" + filePath.Split('\\').Last();
                File.Copy(filePath, transmittedFilePath, true);
                ModelPath transmittedModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(transmittedFilePath);
                transmittedModelPath.UnloadRevitLinks(isSameFolder, folder);
            }
            transmitViewModel.Finisher("TransmitModelsFinished");
        }
    }
}