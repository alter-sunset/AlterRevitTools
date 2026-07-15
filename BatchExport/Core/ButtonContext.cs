using System.Reflection;
using System.Windows.Media.Imaging;
using AlterTools.BatchExport.Core.Commands;
using AlterTools.Utils;

namespace AlterTools.BatchExport.Core;

internal class ButtonContext
{
    [UsedImplicitly] public string Name { get; set; }
    [UsedImplicitly] public string Text { get; set; }
    [UsedImplicitly] public string LibraryName { get; set; }
    [UsedImplicitly] public string ClassName { get; set; }
    [UsedImplicitly] public string ToolTip { get; set; }
    [UsedImplicitly] public string ImageLarge { get; set; }
    [UsedImplicitly] public string ImageSmall { get; set; }
    [UsedImplicitly] public string Panel { get; set; }
    [UsedImplicitly] public bool Availability { get; set; }

    private static string AssemblyFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    private static string CommandsFolder => Path.Combine(AssemblyFolder, "Commands");

    public static List<ButtonContext> GetButtonsContext()
    {
        return JsonHelper<List<ButtonContext>>.DeserializeResource("Resources\\Buttons.json");
    }

    public PushButtonData GetPushButtonData()
    {
        try
        {
            PushButtonData pbData = new(Name,
                GetString(Text),
                Path.Combine(CommandsFolder, LibraryName),
                ClassName)
            {
                ToolTip = GetString(ToolTip),
                Image = GetImage(ImageSmall),
                LargeImage = GetImage(ImageLarge)
            };

            if (Availability)
            {
                pbData.AvailabilityClassName = ClassName + "Availability";
            }

            return pbData;
        }
        catch
        {
            return null;
        }
    }

    private static BitmapFrame GetImage(string imagePath)
    {
        try
        {
            string libraryFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string fullPath = Path.Combine(libraryFolder, imagePath);

            if (!File.Exists(fullPath)) return null;

            using FileStream stream = File.OpenRead(fullPath);
            return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }
        catch
        {
            return null;
        }
    }

    private static string GetString(string name) => Resources.Strings.ResourceManager.GetString(name);
}