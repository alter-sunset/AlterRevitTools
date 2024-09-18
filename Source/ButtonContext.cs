namespace VLS.BatchExportNet.Source
{
    class ButtonContext
    {
        private string _smallImage;
        private string _largeImage;
        private string _toolTip;

        public string SmallImage { get => _smallImage; set => _smallImage = value; }
        public string LargeImage { get => _largeImage; set => _largeImage = value; }
        public string ToolTip { get => _toolTip; set => _toolTip = value; }
    }
}