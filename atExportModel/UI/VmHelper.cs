using System.IO;
using System.Windows;
using AlterTools.Utils.MVVM;
using Autodesk.Revit.DB.Analysis;
using MessageBox = System.Windows.MessageBox;

namespace AlterTools.atExportModel.UI;

public static class VmHelper
{
    /// <summary>
    /// Opens a folder browser dialog and returns the selected path if successful;
    /// otherwise, returns the fallback (original) path.
    /// </summary>
    public static string BrowseFolder(string currentPath)
    {
        using FolderBrowserDialog folderBrowserDialog = new();
        folderBrowserDialog.SelectedPath = currentPath;

        return folderBrowserDialog.ShowDialog() is DialogResult.OK ? folderBrowserDialog.SelectedPath : currentPath;
    }

    public static string BrowseFile(string filePath)
    {
        using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();
        return openFileDialog.ShowDialog() is not DialogResult.OK ? filePath : openFileDialog.FileName;
    }

    public static bool ValidateField(string field, string name)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            MessageBox.Show($"Provide path to export {name}");
            return false;
        }

        if (!IsValidPath(field))
        {
            MessageBox.Show($"Provide valid path to export {name}");
            return false;
        }

        if (Directory.Exists(field)) return true;

        MessageBoxResult errorResult = MessageBox.Show(
            $"There's no such folder for {name} export. Should I create one?",
            $"Invalid {name} path",
            MessageBoxButton.YesNo);
        if (errorResult is MessageBoxResult.Yes)
        {
            Directory.CreateDirectory(field);
            return true;
        }

        MessageBox.Show("Then no cake for you.");
        return false;
    }

    private static bool IsValidPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;

        try
        {
            // 1. Check for characters that Windows strictly forbids in paths
            if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1) return false;

            // 2. Extract the directory portion to catch illegal volume separators (e.g., "C::\")
            string directoryName = Path.GetDirectoryName(path);

            // 3. Ensure the folder name itself doesn't contain illegal file characters
            string folderName = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(folderName) &&
                folderName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) return false;

            // 4. Force a full path resolution to catch malformed structures or system limitations
            string fullPath = Path.GetFullPath(path);

            // 5. Reject relative paths (like "kfjh") if you require a rooted, absolute path
            return Path.IsPathRooted(path);
        }
        catch
        {
            return false;
        }
    }
}