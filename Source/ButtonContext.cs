using Autodesk.Revit.UI;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

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

        public PushButtonData GetPushButtonData()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            try
            {
                PushButtonData pbData = new(Name, Text, location, ClassName)
                {
                    ToolTip = ToolTip,
                    Image = GetImage(ImageSmall),
                    LargeImage = GetImage(ImageLarge)
                };
                if (Availability)
                {
                    pbData.AvailabilityClassName = "VLS.BatchExportNet.Source.CommandAvailability";
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