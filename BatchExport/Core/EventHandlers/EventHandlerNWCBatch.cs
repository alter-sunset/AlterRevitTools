using System;
using System.IO;
using System.Threading;
using System.Windows;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.NWC;
using Autodesk.Revit.UI;
using Newtonsoft.Json;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerNWCBatch : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
    {
        if (iConfigBase is not NWCViewModel nwcVm) return;

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

                NWCForm form = JsonConvert.DeserializeObject<NWCForm>(new StreamReader(file).ReadToEnd());

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

        nwcVm.Finisher("ExportBatchNWCFinished", msg);
    }
}