using AlterTools.BatchExport.Resources;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Utils.Logger;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.NWC;
using Newtonsoft.Json;
using MessageBox = System.Windows.MessageBox;

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
                using FileStream fileStream = File.OpenRead(config.Name);

                NWCForm form = JsonConvert.DeserializeObject<NWCForm>(new StreamReader(fileStream).ReadToEnd());

                nwcVm.DeserializeNWCForm(form);
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

        nwcVm.FinishWork("ExportBatchNWCFinished", msg);
    }
}