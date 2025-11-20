using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Resources;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Utils.Extensions;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Params;

public class ParamsViewModel : ViewModelBase, IConfigParams
{
    private static string DefaultParams => Strings.DefaultParams;

    private string _csvPath = string.Empty;
    private string _paramsNames;

    private RelayCommand _browseCsvCommand;

    public ParamsViewModel(EventHandlerParams eventHandlerParams)
    {
        EventHandlerBase = eventHandlerParams;
        HelpMessage = string.Join(Environment.NewLine,
            Strings.HelpParamsTitle,
            Strings.HelpLoad,
            Strings.HelpConfig,
            Strings.HelpStart);

        ParamsNames = DefaultParams;
    }

    [UsedImplicitly]
    public string ParamsNames
    {
        get => _paramsNames;
        set => SetProperty(ref _paramsNames, value);
    }

    [UsedImplicitly]
    public RelayCommand BrowseCsvCommand => _browseCsvCommand ??= new RelayCommand(_ => BrowseCsv());

    public string[] ParametersNames => _paramsNames.SplitBySemicolon();

    public string CsvPath
    {
        get => _csvPath;
        set => SetProperty(ref _csvPath, value);
    }

    private void BrowseCsv()
    {
        SaveFileDialog saveFileDialog = DialogType.SingleCsv.SaveFileDialog();

        if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

        CsvPath = saveFileDialog.FileName;
    }

    private protected override void LoadList()
    {
        OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        using FileStream file = File.OpenRead(openFileDialog.FileName);

        DeserializeParamsForm(JsonHelper<ParamsForm>.DeserializeConfig(file));
    }

    private void DeserializeParamsForm(ParamsForm form)
    {
        if (form is null) return;

        ParamsNames = string.Join(";", form.ParametersNames);
        CsvPath = form.CsvPath;
        ListBoxItems = [.. form.Files.FilterRevitFiles().Select(DefaultListBoxItem)];
    }

    private protected override void SaveList()
    {
        ParamsForm form = SerializeParamsForm();
        SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

        if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

        string fileName = saveFileDialog.FileName;
        File.Delete(fileName);

        JsonHelper<ParamsForm>.SerializeConfig(form, fileName);
    }

    private ParamsForm SerializeParamsForm()
    {
        return new ParamsForm
        {
            CsvPath = CsvPath,
            ParametersNames = ParametersNames,
            Files = [.. ListBoxItems.Select(item => item.Content.ToString())]
        };
    }

    private protected override void Erase()
    {
        ListBoxItems.Clear();
        ParamsNames = DefaultParams;
        CsvPath = string.Empty;
    }
}