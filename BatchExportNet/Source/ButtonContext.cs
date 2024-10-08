using Autodesk.Revit.UI;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Source
{
    class ButtonContext
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string ClassName { get; set; }
        public string ToolTip { get; set; }
        public string ImageLarge { get; set; }
        public string ImageSmall { get; set; }
        public string Panel { get; set; }
        public bool Availability { get; set; }

        public static ButtonContext[] GetButtonsContext() =>
            JsonHelper<ButtonContext[]>.DeserializeResource("BatchExportNet.Resources.Buttons.json");

        public PushButtonData GetPushButtonData()
        {
            try
            {
                PushButtonData pbData = new(name: Name,
                    text: Text,
                    assemblyName: Assembly.GetExecutingAssembly().Location,
                    className: ClassName)
                {
                    ToolTip = ToolTip,
                    Image = GetImage(ImageSmall),
                    LargeImage = GetImage(ImageLarge)
                };
                if (Availability)
                    pbData.AvailabilityClassName = typeof(CommandAvailability).FullName;

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
                Assembly a = Assembly.GetExecutingAssembly();
                Stream s = a.GetManifestResourceStream(imagePath);
                return BitmapFrame.Create(s);
            }
            catch
            {
                return null;
            }
        }
    }
}