using System.Windows.Forms;

namespace VLS.BatchExportNet.Views
{
    public static class DialogHelper
    {
        private const string EXT_TXT = ".txt";
        private const string EXT_JSON = ".json";
        private const string FILTER_TXT = "Текстовый файл (.txt)|*.txt";
        private const string FILTER_JSON = "Файл JSON (.json)|*.json";
        public static OpenFileDialog OpenFileDialog(this DialogType dialogType)
        {
            bool multiselect = false;
            string defaultExt;
            string filter;
            switch (dialogType)
            {
                case DialogType.SingleText:
                    defaultExt = EXT_TXT;
                    filter = FILTER_TXT;
                    break;
                case DialogType.SingleJson:
                    defaultExt = EXT_JSON;
                    filter = FILTER_JSON;
                    break;
                case DialogType.MultiRevit:
                    multiselect = true;
                    defaultExt = ".rvt";
                    filter = "Revit Files (.rvt)|*.rvt";
                    break;
                default:
                    return null;
            }
            return new()
            {
                Multiselect = multiselect,
                DefaultExt = defaultExt,
                Filter = filter
            };
        }
        public static SaveFileDialog SaveFileDialog(this DialogType dialogType)
        {
            string fileName;
            string defaultExt;
            string filter;
            switch (dialogType)
            {
                case DialogType.SingleJson:
                    fileName = "ConfigBatchExport";
                    defaultExt = EXT_JSON;
                    filter = FILTER_JSON;
                    break;
                case DialogType.RevitList:
                    fileName = "ListOfRVTFiles";
                    defaultExt = EXT_TXT;
                    filter = FILTER_TXT;
                    break;
                default:
                    return null;
            }
            return new()
            {
                FileName = fileName,
                DefaultExt = defaultExt,
                Filter = filter
            };
        }
    }
}