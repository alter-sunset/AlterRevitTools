using System.Reflection;
using System.Windows.Media.Imaging;
using AlterTools.BatchExport.Core.Commands;
using AlterTools.BatchExport.Utils;

namespace AlterTools.BatchExport.Core;

internal class ButtonContext
{
    [UsedImplicitly] public string Name { get; set; }
    [UsedImplicitly] public string Text { get; set; }
    [UsedImplicitly] public string ClassName { get; set; }
    [UsedImplicitly] public string ToolTip { get; set; }
    [UsedImplicitly] public string ImageLarge { get; set; }
    [UsedImplicitly] public string ImageSmall { get; set; }
    [UsedImplicitly] public string Panel { get; set; }
    [UsedImplicitly] public bool Availability { get; set; }

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
                Assembly.GetExecutingAssembly().Location,
                ClassName)
            {
                ToolTip = GetString(ToolTip),
                Image = GetImage(ImageSmall),
                LargeImage = GetImage(ImageLarge)
            };

            if (Availability)
            {
                pbData.AvailabilityClassName = typeof(CommandAvailability).FullName;
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