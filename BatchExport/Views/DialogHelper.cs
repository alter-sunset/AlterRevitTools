using System;
using System.Windows.Forms;

namespace AlterTools.BatchExport.Views
{
    public static class DialogHelper
    {
        private const string EXT_TXT = ".txt";
        private const string EXT_JSON = ".json";
        private const string EXT_RVT = ".rvt";
        private const string EXT_CSV = ".csv";
        private const string FILTER_TXT = "Текстовый файл (.txt)|*.txt";
        private const string FILTER_JSON = "Файл JSON (.json)|*.json";
        private const string FILTER_RVT = "Revit Files (.rvt)|*.rvt";
        private const string FILTER_CSV = "(CSV) разделитель точка с запятой (.csv)|*.csv";
        private const string DEFAULT_FILE_NAME_TXT = "ListOfRVTFiles";
        private const string DEFAULT_FILE_NAME_JSON = "ConfigBatchExport";
        private const string DEFAULT_FILE_NAME_CSV = "ParametersExport";
        private const string EXCEPTION = "Unsupported dialog type";

        public static OpenFileDialog OpenFileDialog(this DialogType dialogType)
        {
            (string defaultExt, string filter, bool multiselect) = GetOpenFileDialogSettings(dialogType);

            return new OpenFileDialog()
            {
                Multiselect = multiselect,
                DefaultExt = defaultExt,
                Filter = filter
            };
        }

        public static SaveFileDialog SaveFileDialog(this DialogType dialogType)
        {
            (string defaultExt, string filter, string fileName) = GetSaveFileDialogSettings(dialogType);

            return new SaveFileDialog()
            {
                FileName = fileName,
                DefaultExt = defaultExt,
                Filter = filter
            };
        }

        private static (string defaultExt, string filter, bool multiselect) GetOpenFileDialogSettings(DialogType dialogType)
        {
            return dialogType switch
            {
                DialogType.SingleText => (EXT_TXT, FILTER_TXT, false),
                DialogType.SingleJson => (EXT_JSON, FILTER_JSON, false),
                DialogType.MultiRevit => (EXT_RVT, FILTER_RVT, true),

                _ => throw new ArgumentOutOfRangeException(nameof(dialogType), EXCEPTION),
            };
        }
        private static (string defaultExt, string filter, string fileName) GetSaveFileDialogSettings(DialogType dialogType)
        {
            return dialogType switch
            {
                DialogType.SingleJson => (EXT_JSON, FILTER_JSON, DEFAULT_FILE_NAME_JSON),
                DialogType.RevitList => (EXT_TXT, FILTER_TXT, DEFAULT_FILE_NAME_TXT),
                DialogType.SingleCsv => (EXT_CSV, FILTER_CSV, DEFAULT_FILE_NAME_CSV),

                _ => throw new ArgumentOutOfRangeException(nameof(dialogType), EXCEPTION),
            };
        }
    }
}