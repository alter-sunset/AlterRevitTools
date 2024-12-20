﻿using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Windows;
using System.Threading;
using Newtonsoft.Json;
using AlterTools.BatchExportNet.Utils;
using AlterTools.BatchExportNet.Views.Base;
using AlterTools.BatchExportNet.Views.NWC;

namespace AlterTools.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerNWC_Batch : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not NWC_ViewModel nwcVM || nwcVM.Configs.Count == 0)
            {
                MessageBox.Show("Загрузите конфиги.");
                return;
            }

            DateTime timeStart = DateTime.Now;

            foreach (Config config in nwcVM.Configs)
            {
                try
                {
                    using FileStream file = File.OpenRead(config.Name);
                    NWCForm form = JsonConvert.DeserializeObject<NWCForm>(new StreamReader(file).ReadToEnd());
                    nwcVM.NWCFormDeserilaizer(form);
                }
                catch
                {
                    continue;
                }
                Logger log = new(nwcVM.FolderPath);

                NWCHelper nwcHelper = new();
                nwcHelper.BatchExportModels(nwcVM, uiApp, ref log);

                Thread.Sleep(1000);

            }

            string msg = $"Задание выполнено. Всего затрачено времени:{DateTime.Now - timeStart}";
            nwcVM.Finisher(id: "ExportBatchNWCFinished", msg);
        }
    }
}