using System;
using System.Windows.Forms;
using AlterTools.BatchExport.Resources;

namespace AlterTools.BatchExport.Views;

public static class DialogHelper
{
    private const string ExtTxt = ".txt";
    private const string ExtJson = ".json";
    private const string ExtRvt = ".rvt";
    private const string ExtCsv = ".csv";
    private const string DefaultFileNameTxt = "ListOfRVTFiles";
    private const string DefaultFileNameJson = "ConfigBatchExport";
    private const string DefaultFileNameCsv = "ParametersExport";
    private const string Exception = "Unsupported dialog type";
    private static string FilterTxt => Strings.Const_FilterTxt;
    private static string FilterJson => Strings.Const_FilterJson;
    private static string FilterRvt => Strings.Const_FilterRvt;
    private static string FilterCsv => Strings.Const_FilterCsv;

    public static OpenFileDialog OpenFileDialog(this DialogType dialogType)
    {
        (string defaultExt, string filter, bool multiselect) = GetOpenFileDialogSettings(dialogType);

        return new OpenFileDialog
        {
            Multiselect = multiselect,
            DefaultExt = defaultExt,
            Filter = filter
        };
    }

    public static SaveFileDialog SaveFileDialog(this DialogType dialogType)
    {
        (string defaultExt, string filter, string fileName) = GetSaveFileDialogSettings(dialogType);

        return new SaveFileDialog
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
            DialogType.SingleText => (ExtTxt, FilterTxt, false),
            DialogType.SingleJson => (ExtJson, FilterJson, false),
            DialogType.MultiRevit => (ExtRvt, FilterRvt, true),

            _ => throw new ArgumentOutOfRangeException(nameof(dialogType), Exception)
        };
    }

    private static (string defaultExt, string filter, string fileName) GetSaveFileDialogSettings(DialogType dialogType)
    {
        return dialogType switch
        {
            DialogType.SingleJson => (ExtJson, FilterJson, DefaultFileNameJson),
            DialogType.RevitList => (ExtTxt, FilterTxt, DefaultFileNameTxt),
            DialogType.SingleCsv => (ExtCsv, FilterCsv, DefaultFileNameCsv),

            _ => throw new ArgumentOutOfRangeException(nameof(dialogType), Exception)
        };
    }
}