using System;
using System.IO;
using System.Threading;
using System.Windows;
using AlterTools.BatchExport.Resources;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Utils.Logger;
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

        if (nwcVm.Configs.Count == 0)
        {
            MessageBox.Show(Strings.AddConfigs);
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

            ILogger log = LoggerFactory.CreateLogger(nwcVm.FolderPath, nwcVm.TurnOffLog);

            NWCHelper nwcHelper = new();
            nwcHelper.BatchExportModels(nwcVm, uiApp, ref log);

            log.Dispose();

            Thread.Sleep(1000);
        }

        string msg = $"{Strings.TaskCompleted}" +
                     $"\n{Strings.TotalTime}{DateTime.Now - timeStart}";

        nwcVm.Finisher("ExportBatchNWCFinished", msg);
    }
}