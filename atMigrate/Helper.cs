using System.IO;
using System.Text.Json;
using AlterTools.Utils.Extensions;
using Autodesk.Revit.DB;
using Application = Autodesk.Revit.ApplicationServices.Application;
using WasBecome = System.Collections.Generic.Dictionary<string, string>;

namespace AlterTools.atMigrate;

public static class Helper
{
    private static string WrongScheme => Resources.Strings.WrongScheme;

    private static WasBecome LoadMigrationConfig(string configPath)
    {
        using FileStream fileStream = File.OpenRead(configPath);

        WasBecome items = JsonSerializer.Deserialize<WasBecome>(new StreamReader(fileStream).ReadToEnd());

        return items ?? throw new InvalidOperationException(WrongScheme);
    }

    private static void CreateDirectoryForFile(string filePath)
    {
        string dir = Path.GetDirectoryName(filePath);
        if (dir is null || Directory.Exists(dir)) return;

        Directory.CreateDirectory(dir);
    }

    public static void ProcessFiles(string configPath, Application app)
    {
        WasBecome items;

        try
        {
            items = LoadMigrationConfig(configPath);
        }
        catch (Exception)
        {
            MessageBox.Show(WrongScheme);
            return;
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
    }

    private static void ProcessMovedFile(string newFile, WasBecome items, Application app)
    {
        using ModelPath newFilePath = ModelPathUtils.ConvertUserVisiblePathToModelPath(newFile);
        newFilePath.ReplaceLinks(items);

        using Document doc = newFilePath.OpenTransmitted(app);

        try
        {
            doc.FreeTheModel();
        }
        catch
        {
            // ignored
        }

        doc.Close();
    }
}