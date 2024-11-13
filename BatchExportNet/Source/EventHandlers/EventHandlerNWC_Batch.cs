using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Text.Json;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;
using VLS.BatchExportNet.Views.NWC;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerNWC_Batch : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfig)
        {
            if (iConfig is not NWC_ViewModel nwcViewModel || nwcViewModel.Configs.Count == 0)
            {
                MessageBox.Show("Загрузите конфиги.");
                return;
            }

            DateTime timeStart = DateTime.Now;

            foreach (Config config in nwcViewModel.Configs)
            {
                try
                {
                    using FileStream file = File.OpenRead(config.Name);
                    NWCForm form = JsonSerializer.Deserialize<NWCForm>(file);
                    nwcViewModel.NWCFormDeserilaizer(form);
                }
                catch
                {
                    continue;
                }
                Logger log = new(nwcViewModel.FolderPath);

                NWCHelper nwcHelper = new();
                nwcHelper.BatchExportModels(nwcViewModel, uiApp, ref log);

                Thread.Sleep(1000);

            }

            string msg = $"Задание выполнено. Всего затрачено времени:{DateTime.Now - timeStart}";
            nwcViewModel.Finisher(id: "ExportBatchNWCFinished", msg);
        }
    }
}