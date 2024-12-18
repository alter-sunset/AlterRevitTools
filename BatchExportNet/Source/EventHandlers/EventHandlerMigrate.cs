using Autodesk.Revit.UI;
using System.Windows;
using System.Collections.Generic;
using AlterTools.BatchExportNet.Utils;
using AlterTools.BatchExportNet.Views.Base;
using AlterTools.BatchExportNet.Views.Migrate;

namespace AlterTools.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerMigrate : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not MigrateViewModel migrateVM || !MigrateHelper.IsConfigPathValid(migrateVM.ConfigPath))
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