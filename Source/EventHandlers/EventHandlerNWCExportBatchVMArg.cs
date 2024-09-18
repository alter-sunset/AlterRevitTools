using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views;
using VLS.BatchExportNet.Views.Migrate;
using VLS.BatchExportNet.Views.NWC;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerNWCExportBatchVMArg : EventHandlerBaseVMArgs
    {
        public override void Execute(UIApplication uiApp, ViewModelBase viewModelBase)
        {
            NWC_ViewModel nwc_ViewModel = viewModelBase as NWC_ViewModel;
            if (nwc_ViewModel.Configs.Count == 0)
            {
                MessageBox.Show("Загрузите конфиги.");
                return;
            }

            DateTime timeStart = DateTime.Now;

            foreach (string config in nwc_ViewModel.Configs)
            {
                try
                {
                    using FileStream file = File.OpenRead(config);
                    NWCForm form = JsonSerializer.Deserialize<NWCForm>(file);
                    nwc_ViewModel.NWCFormDeserilaizer(form);
                }
                catch
                {
                    continue;
                }

                string folder = nwc_ViewModel.FolderPath;
                Logger logger = new(folder);

                NWCHelper.BatchExportModels(uiApp, nwc_ViewModel, ref logger);
                logger.Dispose();
                Thread.Sleep(1000);
            }

            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = "ExportBatchNWCFinished",
                MainContent = $"Задание выполнено. Всего затрачено времени:{DateTime.Now - timeStart}"
            };
            nwc_ViewModel.IsViewEnabled = false;
            taskDialog.Show();
            nwc_ViewModel.IsViewEnabled = true;
        }
    }
}