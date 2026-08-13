using System.IO;
using System.Reflection;

namespace AlterTools.Core;

internal static class CommandLoader
{
    private static string _folder;

    public static IReadOnlyList<Assembly> LoadAll(string subfolder)
    {
        string addinFolder = Path.GetDirectoryName(typeof(CommandLoader).Assembly.Location);

        _folder = Path.Combine(addinFolder, subfolder);

        AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
        AppDomain.CurrentDomain.AssemblyResolve += Resolve;

        List<Assembly> assemblies = [];

        if (!Directory.Exists(_folder)) return assemblies;

        foreach (string file in Directory.GetFiles(_folder, "*.dll"))
        {
            try
            {
                assemblies.Add(Assembly.LoadFrom(file));
            }
            catch (BadImageFormatException)
            {
                // Ignore native DLLs.
            }
        }

        return assemblies;
    }

    private static Assembly Resolve(object sender, ResolveEventArgs args)
    {
        string name = new AssemblyName(args.Name).Name;
        string path = Path.Combine(_folder, name + ".dll");

        return File.Exists(path)
            ? Assembly.LoadFrom(path)
            : null;
    }
}