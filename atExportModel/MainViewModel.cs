using System.Collections.ObjectModel;
using System.Windows.Controls;
using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;
using AlterTools.atExportModel.UI;
using AlterTools.Utils.MVVM;

namespace AlterTools.atExportModel;

public class MainViewModel : NotifyPropertyChanged, IConfigExportMultiple
{
    private readonly ExternalEventHandler _handler;

    public bool ExportRVT { get; set; } = false;
    public bool AsTransmit { get; set; } = true;
    public bool AsCentralModel => !AsTransmit;
    public RvtExportMode RvtExportMode { get; set; }

    public bool ExportNWC { get; set; } = false;
    public IConfigNWC ConfigNWC { get; set; }
    public ViewModelNWC ViewModelNWC { get; }

    public bool ExportIFC { get; set; } = false;
    public IConfigIFC ConfigIFC { get; set; }
    public IConfigIFCAdditionalFields ConfigIFCAdditionalFields { get; set; }
    public ViewModelIFC ViewModelIFC { get; }

    public bool CleanModel { get; set; } = false;
    public IConfigClean ConfigClean { get; set; }
    public ViewModelClean ViewModelClean { get; }

    public string ViewName { get; set; } = "Navisworks";
    public string FolderPathRVT { get; set; }
    public string FolderPathNWC { get; set; }
    public string FolderPathIFC { get; set; }


    private ObservableCollection<string> _inputFiles = [];

    public ObservableCollection<string> InputFiles
    {
        get => _inputFiles;
        set => SetProperty(ref _inputFiles, value);
    }

    private RelayCommand _loadCommand;
    private RelayCommand _deleteCommand;
    private RelayCommand _execute;

    public MainViewModel(ExternalEventHandler handler)
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

    public RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
    public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(param => Delete(param));
    public RelayCommand Execute => _execute ??= new RelayCommand(_ => _handler.Raise(this));

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
}