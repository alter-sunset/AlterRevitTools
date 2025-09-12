using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using AlterTools.BatchExport.Core.Commands;
using AlterTools.BatchExport.Utils;
using Autodesk.Revit.UI;
using JetBrains.Annotations;

namespace AlterTools.BatchExport.Core;

[UsedImplicitly]
internal class ButtonContext
{
    public string Name { get; set; }
    public string Text { get; set; }
    public string ClassName { get; set; }
    public string ToolTip { get; set; }
    public string ImageLarge { get; set; }
    public string ImageSmall { get; set; }
    public string Panel { get; set; }
    public bool Availability { get; set; }

    public static List<ButtonContext> GetButtonsContext()
    {
        return JsonHelper<List<ButtonContext>>.DeserializeResource("AlterTools.BatchExport.Resources.Buttons.json");
    }

    public PushButtonData GetPushButtonData()
    {
        try
        {
            PushButtonData pbData = new(Name, Text, Assembly.GetExecutingAssembly().Location, ClassName)
            {
                ToolTip = ToolTip,
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
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(imagePath);
            return BitmapFrame.Create(stream!);
        }
        catch
        {
            return null;
        }
    }
}