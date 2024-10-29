using Autodesk.Revit.DB;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System;
using System.IO;
using System.Globalization;
using System.Text.Json;
using System.Windows.Forms;
using System.Collections.Generic;
using VLS.BatchExportNet.Utils;
using WasBecome = System.Collections.Generic.Dictionary<string, string>;

namespace VLS.BatchExportNet.Views.Migrate
{
    public static class MigrateHelper
    {
        public static bool IsConfigPathValid(string configPath) => !string.IsNullOrEmpty(configPath)
            && configPath.EndsWith(".json", true, CultureInfo.InvariantCulture);
        public static WasBecome LoadMigrationConfig(string configPath)
        {
            using FileStream fileStream = File.OpenRead(configPath);
            WasBecome items = JsonSerializer.Deserialize<WasBecome>(fileStream);
            return items is null
                ? throw new InvalidOperationException("Неверная схема файла")
                : items;
        }
        public static void CreateDirectoryForFile(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (directory is not null && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
        public static List<string> ProcessFiles(string configPath, Application application)
        {
            WasBecome items;

            try
            {
                items = LoadMigrationConfig(configPath);
            }
            catch (Exception)
            {
                MessageBox.Show("Неверная схема файла");
                return [];
            }

            List<string> failedFiles = new(items.Count);
            List<string> movedFiles = new(items.Count);
            foreach ((string oldFile, string newFile) in items)
            {
                if (!File.Exists(oldFile))
                {
                    failedFiles.Add(oldFile);
                    continue;
                }

                try
                {
                    CreateDirectoryForFile(newFile);
                    File.Copy(oldFile, newFile, true);
                    movedFiles.Add(newFile);
                }
                catch
                {
                    failedFiles.Add(oldFile);
                }
            }
            movedFiles.ForEach(movedFile => ProcessMovedFile(movedFile, items, application));
            return failedFiles;
        }
        private static void ProcessMovedFile(string newFile, WasBecome items, Application application)
        {
            ModelPath newFilePath = ModelPathUtils.ConvertUserVisiblePathToModelPath(newFile);
            newFilePath.ReplaceLinks(items);

            using Document document = newFilePath.OpenTransmitted(application);
            try
            {
                document.FreeTheModel();
            }
            catch { }
            document.Close();
        }
    }
}
