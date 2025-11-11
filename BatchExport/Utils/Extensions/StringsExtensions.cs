using AlterTools.BatchExport.Resources;

namespace AlterTools.BatchExport.Utils.Extensions;

public static class StringsExtensions
{
    public static string RemoveDetach(this string name)
    {
        return name.Replace(Strings.Detached, "");
    }

    public static string[] SplitBySemicolon(this string line)
    {
        return [.. line.Split(';')
            .Select(word => word.Trim())
            .Distinct()
            .Where(word => !string.IsNullOrWhiteSpace(word))];
    }
    
    /// <returns>Unique files with .rvt extension</returns>
    public static IEnumerable<string> FilterRevitFiles(this IEnumerable<string> files)
    {
        return files.Distinct()
            .Where(file => !string.IsNullOrWhiteSpace(file)
                           && Path.GetExtension(file) == ".rvt");
    }
}