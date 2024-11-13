using Autodesk.Revit.UI;
using System.Windows;
using System.Collections.Generic;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;
using VLS.BatchExportNet.Views.Migrate;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerMigrate : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase viewModelBase)
        {
            if (viewModelBase is not MigrateViewModel migrateViewModel || !MigrateHelper.IsConfigPathValid(migrateViewModel.ConfigPath))
            {
                MessageBox.Show("Предоставьте ссылку на конфиг");
                return;
            }
            List<string> failedFiles = MigrateHelper.ProcessFiles(migrateViewModel.ConfigPath, uiApp.Application);

            string msg = failedFiles.Count > 0
                ? $"Задание выполнено.\nСледующие файлы не были скопированы:\n{string.Join("\n", failedFiles)}"
                : "Задание выполнено.";

            migrateViewModel.Finisher(id: "MigrateModelsFinished", msg);
        }
    }
}