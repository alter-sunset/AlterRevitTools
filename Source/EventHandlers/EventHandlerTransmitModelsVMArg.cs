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
    public class EventHandlerTransmitModelsVMArg : EventHandlerBaseVMArgs
    {
        public override void Execute(UIApplication uiApp, IConfigBase viewModelBase)
        {
            TransmitViewModel transmitViewModel = viewModelBase as TransmitViewModel;
            if (!transmitViewModel.IsEverythingFilled())
                return;

            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = [.. transmitViewModel.ListBoxItems];

            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();

                if (!File.Exists(filePath))
                {
                    item.Background = Brushes.Red;
                    continue;
                }

                string folder = transmitViewModel.FolderPath;
                bool isSameFolder = transmitViewModel.IsSameFolder;

                string transmittedFilePath = Path.Combine(folder, Path.GetFileName(filePath));
                File.Copy(filePath, transmittedFilePath, true);
                ModelPath transmittedModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(transmittedFilePath);
                transmittedModelPath.UnloadRevitLinks(folder, isSameFolder);
            }
            transmitViewModel.Finisher(id: "TransmitModelsFinished");
        }
    }
}