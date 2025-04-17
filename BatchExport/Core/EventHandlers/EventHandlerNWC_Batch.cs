using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Windows;
using System.Threading;
using Newtonsoft.Json;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.NWC;

namespace AlterTools.BatchExport.Core.EventHandlers
{
    public class EventHandlerNWC_Batch : EventHandlerBase
    {
        protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not NWC_ViewModel nwcVm) return;

            if (0 == nwcVm.Configs.Count)
            {
                MessageBox.Show("Загрузите конфиги.");
                return;
            }

            DateTime timeStart = DateTime.Now;

            foreach (Config config in nwcVm.Configs)
            {
                try
                {
                    using FileStream file = File.OpenRead(config.Name);

                    NwcForm form = JsonConvert.DeserializeObject<NwcForm>(new StreamReader(file).ReadToEnd());

                    nwcVm.NWCFormDeserilaizer(form);
                }
                catch
                {
                    continue;
                }

                Logger log = new(nwcVm.FolderPath);

                NWCHelper nwcHelper = new();
                nwcHelper.BatchExportModels(nwcVm, uiApp, ref log);

                log.Dispose();

                Thread.Sleep(1000);
            }

            string msg = $"Задание выполнено. Всего затрачено времени:{DateTime.Now - timeStart}";

            nwcVm.Finisher(id: "ExportBatchNWCFinished", msg);
        }
    }
}