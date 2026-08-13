using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using AlterTools.Utils;
using Autodesk.Revit.UI;

namespace AlterTools.Core;

internal class ButtonContext
{
    public string Name { get; set; }
    public string Text { get; set; }
    public string LibraryName { get; set; }
    public string ClassName { get; set; }
    public string ToolTip { get; set; }
    public string ImageLarge { get; set; }
    public string ImageSmall { get; set; }
    public string Panel { get; set; }
    public bool Availability { get; set; }

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