using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Resources;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Params;

public class ParamsViewModel : ViewModelBase, IConfigParams
{
    private static string DefaultParams => Strings.Const_DefaultParams;

    private RelayCommand _browseCsvCommand;

    private string _csvPath = string.Empty;

    private string _paramsNames;

    public ParamsViewModel(EventHandlerParams eventHandlerParams)
    {
        EventHandlerBase = eventHandlerParams;
        HelpMessage = string.Join(Environment.NewLine,
            Strings.Help_ParamsTitle,
            Strings.Help_Load,
            Strings.Help_Config,
            Strings.Help_Start);

        ParamsNames = DefaultParams;
    }

    public string ParamsNames
    {
        get => _paramsNames;
        set => SetProperty(ref _paramsNames, value);
    }

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

    protected override void LoadList()
    {
        OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        using FileStream file = File.OpenRead(openFileDialog.FileName);

        ParamsFormDeserializer(JsonHelper<ParamsForm>.DeserializeConfig(file));
    }

    private void ParamsFormDeserializer(ParamsForm form)
    {
        if (form is null) return;

        ParamsNames = string.Join(";", form.ParametersNames);
        CsvPath = form.CsvPath;
        ListBoxItems = [.. form.Files.FilterRevitFiles().Select(DefaultListBoxItem)];
    }

    protected override void SaveList()
    {
        ParamsForm form = ParamsFormSerializer();
        SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

        if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

        string fileName = saveFileDialog.FileName;
        File.Delete(fileName);

        JsonHelper<ParamsForm>.SerializeConfig(form, fileName);
    }

    private ParamsForm ParamsFormSerializer()
    {
        return new ParamsForm
        {
            CsvPath = CsvPath,
            ParametersNames = ParametersNames,
            Files = [.. ListBoxItems.Select(item => item.Content.ToString())]
        };
    }

    protected override void Erase()
    {
        ListBoxItems.Clear();
        ParamsNames = DefaultParams;
        CsvPath = string.Empty;
    }
}