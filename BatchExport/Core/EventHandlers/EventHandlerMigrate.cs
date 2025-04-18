using Autodesk.Revit.UI;
using System.Windows;
using System.Collections.Generic;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Migrate;

namespace AlterTools.BatchExport.Core.EventHandlers
{
    public class EventHandlerMigrate : EventHandlerBase
    {
        protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not MigrateViewModel migrateVm) return;

            if (!MigrateHelper.IsConfigPathValid(migrateVm.ConfigPath))
            {
                MessageBox.Show("Предоставьте ссылку на конфиг");
                return;
            }

            List<string> failedFiles = MigrateHelper.ProcessFiles(migrateVm.ConfigPath, uiApp.Application);

            string msg = failedFiles.Count > 0
                            ? $"Задание выполнено.\nСледующие файлы не были скопированы:\n{string.Join("\n", failedFiles)}"
                            : "Задание выполнено.";

            migrateVm.Finisher(id: "MigrateModelsFinished", msg);
        }
    }
}