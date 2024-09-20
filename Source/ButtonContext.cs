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
        public string Panel { get; set; }
        public bool Availability { get; set; }

        public BitmapFrame GetImage(string BASE, bool isSmall)
        {
            string end;
            if (isSmall)
            {
                end = "_16.png";
            }
            else
            {
                end = ".png";
            }
            string name = BASE + "Resources." + Name.ToLower() + end;

            try
            {
                Assembly a = Assembly.GetExecutingAssembly();
                Stream s = a.GetManifestResourceStream(name);
                return BitmapFrame.Create(s);
            }
            catch
            {
                return null;
            }
        }
        public PushButtonData GetPushButtonData(string BASE)
        {
            try
            {
                return new(Name, Text,
                    Assembly.GetExecutingAssembly().Location,
                    BASE + "Source." + ClassName)
                {
                    ToolTip = ToolTip,
                    Image = GetImage(BASE, true),
                    LargeImage = GetImage(BASE, false)
                };
            }
            catch
            {
                return null;
            }
        }
    }
}