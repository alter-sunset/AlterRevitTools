using System.IO;
using System.Reflection;
using System.Text.Json;
using MessageBox = System.Windows.MessageBox;

namespace AlterTools.Utils;

public static class JsonHelper<T>
{
    public static T DeserializeResource(string path)
    {
        try
        {
            string libraryFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string fullPath = Path.Combine(libraryFolder, path);

            if (!File.Exists(fullPath)) return default;

            string json = File.ReadAllText(fullPath);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }

    public static T DeserializeConfig(FileStream file)
    {
        return HandleSerialization(() => JsonSerializer.Deserialize<T>(new StreamReader(file).ReadToEnd()));
    }

    public static void SerializeConfig(T value, string path)
    {
        HandleSerialization(() =>
        {
            using FileStream stream = new(path, FileMode.Create, FileAccess.Write, FileShare.None);
            using StreamWriter writer = new(stream);

            writer.Write(JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true }));

            return default;
        });
    }

    private static T HandleSerialization(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{Resources.Strings.WrongScheme}\n{ex.Message}");
            return default;
        }
    }
}