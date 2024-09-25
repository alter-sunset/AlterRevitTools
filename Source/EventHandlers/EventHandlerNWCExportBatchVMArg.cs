using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;
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

                NWCHelper nwcHelper = new();
                nwcHelper.BatchExportModels(nwc_ViewModel, uiApp, ref logger);

                logger.Dispose();
                Thread.Sleep(1000);
            }

            string msg = $"Задание выполнено. Всего затрачено времени:{DateTime.Now - timeStart}";
            nwc_ViewModel.Finisher(id: "ExportBatchNWCFinished", msg);
        }
    }
}