﻿using Autodesk.Revit.DB;
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;
using AlterTools.BatchExport.Utils;
using Application = Autodesk.Revit.ApplicationServices.Application;
using WasBecome = System.Collections.Generic.Dictionary<string, string>;

namespace AlterTools.BatchExport.Views.Migrate
{
    public static class MigrateHelper
    {
        private const string WRONG_SCHEME = "Неверная схема файла";

        public static bool IsConfigPathValid(string configPath)
        {
            return !string.IsNullOrEmpty(configPath) && (".json" == Path.GetExtension(configPath));
        }

        public static WasBecome LoadMigrationConfig(string configPath)
        {
            using FileStream fileStream = File.OpenRead(configPath);

            WasBecome items = JsonConvert.DeserializeObject<WasBecome>(new StreamReader(fileStream).ReadToEnd());

            return items ?? throw new InvalidOperationException(WRONG_SCHEME);
        }

        public static void CreateDirectoryForFile(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            if ((null != dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static List<string> ProcessFiles(string configPath, Application app)
        {
            WasBecome items;

            try
            {
                items = LoadMigrationConfig(configPath);
            }
            catch (Exception)
            {
                MessageBox.Show(WRONG_SCHEME);
                return new List<string>();
            }

            List<string> failedFiles = new(items.Count);
            List<string> movedFiles = new(items.Count);

            foreach (KeyValuePair<string, string> item in items)
            {
                string oldFile = item.Key;
                string newFile = item.Value;

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

            movedFiles.ForEach(movedFile => ProcessMovedFile(movedFile, items, app));

            return failedFiles;
        }

        private static void ProcessMovedFile(string newFile, WasBecome items, Application app)
        {
            ModelPath newFilePath = ModelPathUtils.ConvertUserVisiblePathToModelPath(newFile);
            newFilePath.ReplaceLinks(items);

            using Document doc = newFilePath.OpenTransmitted(app);

            try
            {
                doc.FreeTheModel();
            }
            catch { }

            doc.Close();
        }
    }
}