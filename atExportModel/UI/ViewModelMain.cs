using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;
using AlterTools.Utils.MVVM;
using MessageBox = System.Windows.MessageBox;

namespace AlterTools.atExportModel.UI;

public class ViewModelMain : NotifyPropertyChanged, IConfigExportMultiple
{
    private readonly ExternalEventHandler _handler;

    private bool _exportRVT;

    public bool ExportRVT
    {
        get => _exportRVT;
        set => SetProperty(ref _exportRVT, value);
    }

    private RvtExportMode _rvtExportMode = RvtExportMode.Transmit;

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

    private bool _exportNWC;

    public bool ExportNWC
    {
        get => _exportNWC;
        set => SetProperty(ref _exportNWC, value);
    }

    public IConfigNWC ConfigNWC { get; set; }
    public ViewModelNWC ViewModelNWC { get; }

    private bool _exportIFC;

    public bool ExportIFC
    {
        get => _exportIFC;
        set => SetProperty(ref _exportIFC, value);
    }

    public IConfigIFC ConfigIFC { get; set; }
    public IConfigIFCAdditionalFields ConfigIFCAdditionalFields { get; set; }
    public ViewModelIFC ViewModelIFC { get; }

    public bool CleanModel { get; set; } = false;
    public IConfigClean ConfigClean { get; set; }
    public ViewModelClean ViewModelClean { get; }

    private string _viewName = "Navisworks";
    private string _folderPathNWC = string.Empty;
    private string _folderPathIFC = string.Empty;
    private string _folderPathRVT = string.Empty;

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


    private ObservableCollection<string> _inputFiles = [];

    public ObservableCollection<string> InputFiles
    {
        get => _inputFiles;
        set => SetProperty(ref _inputFiles, value);
    }

    private RelayCommand _browseNWCFolderCommand;
    private RelayCommand _browseIFCFolderCommand;
    private RelayCommand _browseRVTFolderCommand;
    private RelayCommand _settingsNWCCommand;
    private RelayCommand _settingsIFCCommand;
    private RelayCommand _settingsCleanCommand;
    private RelayCommand _loadCommand;
    private RelayCommand _deleteCommand;
    private RelayCommand _executeCommand;

    public ViewModelMain(ExternalEventHandler handler)
    {
        _handler = handler;

        ConfigNWC = new ConfigNWC();
        ViewModelNWC = new ViewModelNWC(ConfigNWC);

        ConfigIFC = new ConfigIFC();
        ConfigIFCAdditionalFields = (IConfigIFCAdditionalFields)ConfigIFC;
        ViewModelIFC = new ViewModelIFC((ConfigIFC)ConfigIFC);

        ConfigClean = new ConfigClean();
        ViewModelClean = new ViewModelClean((ConfigClean)ConfigClean);
    }

    public RelayCommand SettingsNWCCommand => _settingsNWCCommand ??= new RelayCommand(_ => OpenSettingsNWC());
    public RelayCommand SettingsIFCCommand => _settingsIFCCommand ??= new RelayCommand(_ => OpenSettingIFC());
    public RelayCommand SettingsCleanCommand => _settingsCleanCommand ??= new RelayCommand(_ => OpenSettingsClean());

    public RelayCommand BrowseFolderNWCCommand => _browseNWCFolderCommand ??= new RelayCommand(_ => BrowseFolderNWC());
    public RelayCommand BrowseFolderIFCCommand => _browseIFCFolderCommand ??= new RelayCommand(_ => BrowseFolderIFC());
    public RelayCommand BrowseFolderRVTCommand => _browseRVTFolderCommand ??= new RelayCommand(_ => BrowseFolderRVT());

    public RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
    public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(param => Delete(param));
    public RelayCommand ExecuteCommand => _executeCommand ??= new RelayCommand(_ => Execute());

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

    private void BrowseFolderNWC() => FolderPathNWC = VmHelper.BrowseFolder(FolderPathNWC);
    private void BrowseFolderIFC() => FolderPathIFC = VmHelper.BrowseFolder(FolderPathIFC);
    private void BrowseFolderRVT() => FolderPathRVT = VmHelper.BrowseFolder(FolderPathRVT);

    private void Load()
    {
        using OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        HashSet<string> existingFiles = [.. InputFiles];

        IEnumerable<string> files = openFileDialog.FileNames
            .Distinct()
            .Where(file => !existingFiles.Contains(file));

        foreach (string file in files)
        {
            InputFiles.Add(file);
        }
    }

    private void Delete(object parameter)
    {
        // If the parameter is null or wrong type, exit safely
        if (parameter is not System.Collections.IList selectedItems) return;

        // Safely copy the selected strings to a temporary list
        List<string> itemsToDelete = selectedItems.Cast<string>().ToList();

        // Remove the strings directly from your collection
        foreach (string item in itemsToDelete)
        {
            InputFiles.Remove(item);
        }
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
            MessageBox.Show("Provide at least one file!");
            return false;
        }

        if (!_exportNWC && !_exportIFC && !_exportRVT)
        {
            MessageBox.Show("Select at least one export mode!");
            return false;
        }

        if (_exportNWC && !VmHelper.ValidateField(FolderPathNWC, "NWC")) return false;
        if (_exportIFC && !VmHelper.ValidateField(FolderPathIFC, "IFC")) return false;
        if (_exportRVT && !VmHelper.ValidateField(FolderPathRVT, "RVT")) return false;

        return true;
    }
}