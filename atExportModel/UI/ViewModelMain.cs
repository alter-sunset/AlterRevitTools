using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;
using AlterTools.Resources;
using AlterTools.Utils;
using AlterTools.Utils.MVVM;
using MessageBox = System.Windows.MessageBox;

namespace AlterTools.atExportModel.UI;

public class ViewModelMain : NotifyPropertyChanged, IConfigExportMultiple
{
    private readonly ExternalEventHandler _handler;
    private RelayCommand _browseIFCFolderCommand;

    private RelayCommand _browseNWCFolderCommand;
    private RelayCommand _browseRVTFolderCommand;

    private bool _cleanModel;

    private ConfigClean _configClean;

    private ConfigIFC _configIFC;
    private ConfigIFCAdd _configIFCAdd;

    private ConfigNWC _configNWC;
    private RelayCommand _deleteCommand;
    private RelayCommand _executeCommand;
    private RelayCommand _exportCommand;

    private bool _exportIFC;

    private bool _exportNWC;

    private bool _exportRVT;
    private string _folderPathIFC = string.Empty;
    private string _folderPathNWC = string.Empty;
    private string _folderPathRVT = string.Empty;
    private RelayCommand _importCommand;

    private ObservableCollection<string> _inputFiles = [];
    private RelayCommand _loadCommand;

    private RvtExportMode _rvtExportMode = RvtExportMode.Transmit;
    private RelayCommand _settingsCleanCommand;
    private RelayCommand _settingsIFCCommand;
    private RelayCommand _settingsNWCCommand;

    private string _viewName = "Navisworks";

    public ViewModelMain(ExternalEventHandler handler)
    {
        _handler = handler;

        ConfigNWC = new ConfigNWC();
        ViewModelNWC = new ViewModelNWC(ConfigNWC);

        ConfigIFC = new ConfigIFC();
        ConfigIFCAdditionalFields = new ConfigIFCAdd();
        ViewModelIFC = new ViewModelIFC(ConfigIFC, ConfigIFCAdditionalFields);

        ConfigClean = new ConfigClean();
        ViewModelClean = new ViewModelClean(ConfigClean);
    }

    public ViewModelNWC ViewModelNWC { get; }

    public ViewModelIFC ViewModelIFC { get; }

    public ViewModelClean ViewModelClean { get; }

    public RelayCommand SettingsNWCCommand => _settingsNWCCommand ??= new RelayCommand(_ => OpenSettingsNWC());
    public RelayCommand SettingsIFCCommand => _settingsIFCCommand ??= new RelayCommand(_ => OpenSettingIFC());
    public RelayCommand SettingsCleanCommand => _settingsCleanCommand ??= new RelayCommand(_ => OpenSettingsClean());

    public RelayCommand BrowseFolderNWCCommand => _browseNWCFolderCommand ??= new RelayCommand(_ => BrowseFolderNWC());
    public RelayCommand BrowseFolderIFCCommand => _browseIFCFolderCommand ??= new RelayCommand(_ => BrowseFolderIFC());
    public RelayCommand BrowseFolderRVTCommand => _browseRVTFolderCommand ??= new RelayCommand(_ => BrowseFolderRVT());

    public RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
    public RelayCommand ImportCommand => _importCommand ??= new RelayCommand(_ => ImportConfig());
    public RelayCommand ExportCommand => _exportCommand ??= new RelayCommand(_ => ExportConfig());
    public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(param => Delete(param));
    public RelayCommand ExecuteCommand => _executeCommand ??= new RelayCommand(_ => Execute());

    public bool ExportRVT
    {
        get => _exportRVT;
        set => SetProperty(ref _exportRVT, value);
    }

    public RvtExportMode RvtExportMode
    {
        get => _rvtExportMode;
        set
        {
            if (_rvtExportMode == value)
                return;

            _rvtExportMode = value;
            OnPropertyChanged();
        }
    }

    public bool ExportNWC
    {
        get => _exportNWC;
        set => SetProperty(ref _exportNWC, value);
    }

    public ConfigNWC ConfigNWC
    {
        get => _configNWC;
        set
        {
            SetProperty(ref _configNWC, value);
            if (ViewModelNWC is not null) ViewModelNWC.Config = value;
        }
    }

    public bool ExportIFC
    {
        get => _exportIFC;
        set => SetProperty(ref _exportIFC, value);
    }

    public ConfigIFC ConfigIFC
    {
        get => _configIFC;
        set
        {
            SetProperty(ref _configIFC, value);
            if (ViewModelIFC is not null) ViewModelIFC.Config = value;
        }
    }

    public ConfigIFCAdd ConfigIFCAdditionalFields
    {
        get => _configIFCAdd;
        set
        {
            SetProperty(ref _configIFCAdd, value);
            if (ViewModelIFC is not null) ViewModelIFC.ConfigAdd = value;
        }
    }

    public bool CleanModel
    {
        get => _cleanModel;
        set => SetProperty(ref _cleanModel, value);
    }

    public ConfigClean ConfigClean
    {
        get => _configClean;
        set
        {
            SetProperty(ref _configClean, value);
            if (ViewModelClean is null) return;
            ViewModelClean.Config = value;
            ViewModelClean.Config.ConfigRemoveViews = value.ConfigRemoveViews;
        }
    }

    public string ViewName
    {
        get => _viewName;
        set => SetProperty(ref _viewName, value);
    }

    public string FolderPathRVT
    {
        get => _folderPathRVT;
        set => SetProperty(ref _folderPathRVT, value);
    }

    public string FolderPathNWC
    {
        get => _folderPathNWC;
        set => SetProperty(ref _folderPathNWC, value);
    }

    public string FolderPathIFC
    {
        get => _folderPathIFC;
        set => SetProperty(ref _folderPathIFC, value);
    }

    public ObservableCollection<string> InputFiles
    {
        get => _inputFiles;
        set => SetProperty(ref _inputFiles, value);
    }

    private void OpenSettingsNWC()
    {
        WindowNWC window = new(ViewModelNWC);
        window.ShowDialog();
    }

    private void OpenSettingIFC()
    {
        WindowIFC window = new(ViewModelIFC);
        window.ShowDialog();
    }

    private void OpenSettingsClean()
    {
        WindowClean window = new(ViewModelClean);
        window.ShowDialog();
    }

    private void BrowseFolderNWC()
    {
        FolderPathNWC = VmHelper.BrowseFolder(FolderPathNWC);
    }

    private void BrowseFolderIFC()
    {
        FolderPathIFC = VmHelper.BrowseFolder(FolderPathIFC);
    }

    private void BrowseFolderRVT()
    {
        FolderPathRVT = VmHelper.BrowseFolder(FolderPathRVT);
    }

    private void Load()
    {
        using OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        HashSet<string> existingFiles = [.. InputFiles];

        IEnumerable<string> files = openFileDialog.FileNames
            .Distinct()
            .Where(file => !existingFiles.Contains(file));

        foreach (string file in files) InputFiles.Add(file);
    }

    private void ImportConfig()
    {
        using OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        using FileStream file = File.OpenRead(openFileDialog.FileName);

        try
        {
            DeserializeConfig(JsonHelper<ConfigExportMultiple>.DeserializeConfig(file));
        }
        catch
        {
            MessageBox.Show(Strings.InvalidConfigFile);
        }
    }

    private void DeserializeConfig(ConfigExportMultiple config)
    {
        if (config == null) return;

        // RVT Export Settings
        ExportRVT = config.ExportRVT;
        RvtExportMode = config.RvtExportMode;
        FolderPathRVT = config.FolderPathRVT;

        // NWC Export Settings
        ExportNWC = config.ExportNWC;
        ConfigNWC = config.ConfigNWC;
        FolderPathNWC = config.FolderPathNWC;

        // IFC Export Settings
        ExportIFC = config.ExportIFC;
        ConfigIFC = config.ConfigIFC;
        ConfigIFCAdditionalFields = config.ConfigIFCAdditionalFields;
        FolderPathIFC = config.FolderPathIFC;

        // Model Cleaning Settings
        CleanModel = config.CleanModel;
        ConfigClean = config.ConfigClean;

        // General Settings
        ViewName = config.ViewName;

        // Input Files Collection
        InputFiles.Clear();
        foreach (string file in config.InputFiles) InputFiles.Add(file);
    }

    private void ExportConfig()
    {
        ConfigExportMultiple configExportMultiple = new(this);
        using SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

        if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

        string fileName = saveFileDialog.FileName;
        File.Delete(fileName);

        JsonHelper<ConfigExportMultiple>.SerializeConfig(configExportMultiple, fileName);
    }

    private void Delete(object parameter)
    {
        // If the parameter is null or wrong type, exit safely
        if (parameter is not IList selectedItems) return;

        // Safely copy the selected strings to a temporary list
        List<string> itemsToDelete = selectedItems.Cast<string>().ToList();

        // Remove the strings directly from your collection
        foreach (string item in itemsToDelete) InputFiles.Remove(item);
    }

    private void Execute()
    {
        if (!ValidateInputs()) return;

        _handler.Raise(this);
    }

    private bool ValidateInputs()
    {
        if (!_inputFiles.Any())
        {
            MessageBox.Show(Strings.ProvideFilesError);
            return false;
        }

        if (!_exportNWC && !_exportIFC && !_exportRVT)
        {
            MessageBox.Show(Strings.SelectExportModeError);
            return false;
        }

        if (_exportNWC && !VmHelper.ValidateField(FolderPathNWC, "NWC")) return false;
        if (_exportIFC && !VmHelper.ValidateField(FolderPathIFC, "IFC")) return false;
        if (_exportRVT && !VmHelper.ValidateField(FolderPathRVT, "RVT")) return false;

        return true;
    }
}