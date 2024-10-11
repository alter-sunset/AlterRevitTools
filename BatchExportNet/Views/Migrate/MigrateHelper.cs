using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using VLS.BatchExportNet.Utils;
using WasBecome = System.Collections.Generic.Dictionary<string, string>;

namespace VLS.BatchExportNet.Views.Migrate
{
    public static class MigrateHelper
    {
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
        public static void ProcessMovedFiles(IEnumerable<string> movedFiles, WasBecome items, Application application)
        {
            foreach (string newFile in movedFiles)
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
}
