using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System.IO;
using System.Windows;
using System.Globalization;
using System.Collections.Generic;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Migrate;
using WasBecome = System.Collections.Generic.Dictionary<string, string>;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerMigrateModelsVMArg : EventHandlerBaseVMArgs
    {
        public override void Execute(UIApplication uiApp, IConfigBase viewModelBase)
        {
            MigrateViewModel migrateViewModel = viewModelBase as MigrateViewModel;
            if (string.IsNullOrEmpty(migrateViewModel.ConfigPath)
                || !migrateViewModel.ConfigPath.EndsWith(".json", true, CultureInfo.InvariantCulture))
            {
                MessageBox.Show("Предоставьте ссылку на конфиг");
                return;
            }

            List<string> movedFiles = [];
            List<string> failedFiles = [];

            WasBecome items;
            try
            {
                items = MigrateHelper.LoadMigrationConfig(migrateViewModel.ConfigPath);
            }
            catch
            {
                MessageBox.Show("Неверная схема файла");
                return;
            }

            using Application application = uiApp.Application;

            foreach (string oldFile in items.Keys)
            {
                if (!File.Exists(oldFile))
                {
                    failedFiles.Add(oldFile);
                    continue;
                }

                string newFile = items[oldFile];

                try
                {
                    MigrateHelper.CreateDirectoryForFile(newFile);
                    File.Copy(oldFile, newFile, true);
                    movedFiles.Add(newFile);
                }
                catch
                {
                    failedFiles.Add(oldFile);
                }
            }

            MigrateHelper.ProcessMovedFiles(movedFiles, items, application);

            string msg = failedFiles.Count > 0
                ? $"Задание выполнено.\nСледующие файлы не были скопированы:\n{string.Join("\n", failedFiles)}"
                : "Задание выполнено.";

            migrateViewModel.Finisher(id: "MigrateModelsFinished", msg);
        }
    }
}