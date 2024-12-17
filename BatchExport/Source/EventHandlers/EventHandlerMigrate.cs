﻿using Autodesk.Revit.UI;
using System.Windows;
using System.Collections.Generic;
using VLS.BatchExport.Utils;
using VLS.BatchExport.Views.Base;
using VLS.BatchExport.Views.Migrate;

namespace VLS.BatchExport.Source.EventHandlers
{
    public class EventHandlerMigrate : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (!(iConfigBase is MigrateViewModel migrateVM) || !MigrateHelper.IsConfigPathValid(migrateVM.ConfigPath))
            {
                MessageBox.Show("Предоставьте ссылку на конфиг");
                return;
            }
            List<string> failedFiles = MigrateHelper.ProcessFiles(migrateVM.ConfigPath, uiApp.Application);

            string msg = failedFiles.Count > 0
                ? $"Задание выполнено.\nСледующие файлы не были скопированы:\n{string.Join("\n", failedFiles)}"
                : "Задание выполнено.";

            migrateVM.Finisher(id: "MigrateModelsFinished", msg);
        }
    }
}