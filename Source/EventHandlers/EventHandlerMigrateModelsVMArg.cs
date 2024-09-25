using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Globalization;
using System.Collections.Generic;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Migrate;
using WasBecome = System.Collections.Generic.Dictionary<string, string>;
using VLS.BatchExportNet.Views.Base;
using System.Text.Json;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerMigrateModelsVMArg : EventHandlerBaseVMArgs
    {
        public override void Execute(UIApplication uiApp, ViewModelBase viewModelBase)
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
            using (FileStream file = File.OpenRead(migrateViewModel.ConfigPath))
            {
                try
                {
                    items = JsonSerializer.Deserialize<WasBecome>(file);
                }
                catch
                {
                    MessageBox.Show("Неверная схема файла");
                    return;
                }
            }

            using Application application = uiApp.Application;

            foreach (string oldFile in items.Keys)
            {
                if (!File.Exists(oldFile))
                {
                    failedFiles.Add(oldFile);
                    continue;
                }

                string newFile = items.FirstOrDefault(e => e.Key == oldFile).Value;

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFile));
                    File.Copy(oldFile, newFile, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    failedFiles.Add(oldFile);
                    continue;
                }

                movedFiles.Add(newFile);
            }

            foreach (string newFile in movedFiles)
            {
                ModelPath newFilePath = ModelPathUtils.ConvertUserVisiblePathToModelPath(newFile);
                newFilePath.ReplaceLinks(items);

                using Document document = newFilePath.OpenTransmitted(application);

                try
                {
                    document.FreeTheModel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                document.Close();
            }

            string msg = failedFiles.Count > 0
                ? $"Задание выполнено.\nСледующие файлы не были скопированы:\n{string.Join("\n", failedFiles)}"
                : "Задание выполнено.";

            migrateViewModel.Finisher(id: "MigrateModelsFinished", msg);
        }
    }
}