using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using MessageBox = System.Windows.MessageBox;

namespace AlterTools.Utils;

public static class JsonHelper<T>
{
    // Centralize options (e.g., enable case insensitivity, converters, or pretty printing)
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Change or remove based on needs
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public static T DeserializeResource(string path)
    {
        try
        {
            string libraryFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(libraryFolder)) return default;

            string fullPath = Path.Combine(libraryFolder, path);

            if (!File.Exists(fullPath)) return default;

            // Open stream directly for faster, lower-allocation parsing
            using FileStream stream = File.OpenRead(fullPath);
            return JsonSerializer.Deserialize<T>(stream, DefaultOptions);
        }
        catch
        {
            return default;
        }
    }

    public static T DeserializeConfig(FileStream file)
    {
        return HandleSerialization(() => JsonSerializer.Deserialize<T>(file, DefaultOptions));
    }

    public static void SerializeConfig(T value, string path)
    {
        // Change Func<T> handler pattern to Action to match type-free writing tasks
        try
        {
            using FileStream stream = new(path, FileMode.Create, FileAccess.Write, FileShare.None);

            // System.Text.Json writes directly to streams efficiently
            JsonSerializer.Serialize(stream, value, DefaultOptions);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{Resources.Strings.WrongScheme}\n{ex.Message}");
        }
    }

    private static T HandleSerialization(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            // Fallback UI reporting
            MessageBox.Show($"{Resources.Strings.WrongScheme}\n{ex.Message}");
            return default;
        }
    }
}