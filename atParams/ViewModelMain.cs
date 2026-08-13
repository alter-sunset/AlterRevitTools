using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using AlterTools.Resources;
using AlterTools.Utils;
using AlterTools.Utils.Extensions;
using AlterTools.Utils.MVVM;

namespace AlterTools.atParams;

public class ViewModelMain : NotifyPropertyChanged, IConfigParams
{
    private readonly ExternalEventHandler _handler;

    private string _paramsNames;
    private string _csvPath = string.Empty;
    private ObservableCollection<string> _files = [];

    private RelayCommand _browseCsvCommand;
    private RelayCommand _loadCommand;
    private RelayCommand _loadListCommand;
    private RelayCommand _saveListCommand;
    private RelayCommand _deleteCommand;
    private RelayCommand _executeCommand;

    public ViewModelMain(ExternalEventHandler eventHandler)
    {
        _handler = eventHandler;
        ParamsNames = DefaultParams;
    }

    private static string DefaultParams => Strings.DefaultParams;

    public string ParamsNames
    {
        get => _paramsNames;
        set => SetProperty(ref _paramsNames, value);
    }

    public RelayCommand BrowseCsvCommand => _browseCsvCommand ??= new RelayCommand(_ => BrowseCsv());
    public RelayCommand LoadListCommand => _loadListCommand ??= new RelayCommand(_ => LoadConfig());
    public RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
    public RelayCommand SaveListCommand => _saveListCommand ??= new RelayCommand(_ => SaveConfig());
    public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(param => Delete(param));
    public RelayCommand ExecuteCommand => _executeCommand ??= new RelayCommand(_ => Execute());

    public string[] ParametersNames
    {
        get => _paramsNames.SplitBySemicolon();
        set { }
    }

    public string CsvPath
    {
        get => _csvPath;
        set => SetProperty(ref _csvPath, value);
    }

    public ObservableCollection<string> Files
    {
        get => _files;
        set => SetProperty(ref _files, value);
    }

    private void BrowseCsv()
    {
        SaveFileDialog saveFileDialog = DialogType.SingleCsv.SaveFileDialog();

        if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

        CsvPath = saveFileDialog.FileName;
    }

    private void LoadConfig()
    {
        OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        using FileStream file = File.OpenRead(openFileDialog.FileName);

        try
        {
            DeserializeParamsForm(JsonHelper<ConfigParams>.DeserializeConfig(file));
        }
        catch
        {
            MessageBox.Show(Strings.InvalidConfigFile);
        }
    }

    private void DeserializeParamsForm(ConfigParams form)
    {
        if (form is null) return;

        ParamsNames = string.Join(";", form.ParametersNames);
        CsvPath = form.CsvPath;
        Files = [.. form.Files.FilterRevitFiles()];
    }

    private void SaveConfig()
    {
        ConfigParams form = SerializeParamsForm();
        SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

        if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

        string fileName = saveFileDialog.FileName;
        File.Delete(fileName);

        JsonHelper<ConfigParams>.SerializeConfig(form, fileName);
    }

    private ConfigParams SerializeParamsForm()
    {
        return new ConfigParams
        {
            CsvPath = CsvPath,
            ParametersNames = ParametersNames,
            Files = Files
        };
    }

    private void Load()
    {
        using OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        HashSet<string> existingFiles = [.. Files];

        IEnumerable<string> files = openFileDialog.FileNames
            .Distinct()
            .Where(file => !existingFiles.Contains(file));

        foreach (string file in files) Files.Add(file);
    }

    private void Delete(object parameter)
    {
        // If the parameter is null or wrong type, exit safely
        if (parameter is not IList selectedItems) return;

        // Safely copy the selected strings to a temporary list
        List<string> itemsToDelete = selectedItems.Cast<string>().ToList();

        // Remove the strings directly from your collection
        foreach (string item in itemsToDelete) Files.Remove(item);
    }

    private void Execute()
    {
        if (!ValidateInputs()) return;

        _handler.Raise(this);
    }

    private bool ValidateInputs()
    {
        if (Files.Any()) return true;

        MessageBox.Show(Strings.ProvideFilesError);
        return false;
    }
}