using System.Collections.ObjectModel;
using System.Windows.Controls;
using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;
using AlterTools.atExportModel.Windows;
using AlterTools.Utils.MVVM;

namespace AlterTools.atExportModel;

public class MainViewModel : NotifyPropertyChanged, IConfigExport
{
    private readonly ExternalEventHandler _handler;

    public bool ExportRVT { get; set; } = false;
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

    public string ViewName { get; set; } = "Navisworks";
    public string FolderPathRVT { get; set; }
    public string FolderPathNWC { get; set; }
    public string FolderPathIFC { get; set; }

    public string[] InputFiles
    {
        get => _filesCollection.Select(item => item.Content.ToString()).ToArray();
        set => _filesCollection = [.. value.Select(e => new ListBoxItem { Content = e })];
    }

    private ObservableCollection<ListBoxItem> _filesCollection = [];

    public ObservableCollection<ListBoxItem> FilesCollection
    {
        get => _filesCollection;
        set => SetProperty(ref _filesCollection, value);
    }

    private RelayCommand _loadCommand;
    private RelayCommand _deleteCommand;

    public MainViewModel(ExternalEventHandler handler)
    {
        _handler = handler;

        ConfigNWC = new ConfigNWC();
        ViewModelNWC = new ViewModelNWC(ConfigNWC);

        ConfigIFC = new ConfigIFC();
        ConfigIFCAdditionalFields = (IConfigIFCAdditionalFields)ConfigIFC;
        ViewModelIFC = new ViewModelIFC((ConfigIFC)ConfigIFC);
    }

    public RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
    public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(param => Delete(param));

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
            FilesCollection.Add(new ListBoxItem { Content = file });
        }
    }

    private void Delete(object parameter)
    {
        // If the parameter is null or wrong type, exit safely
        if (parameter is not System.Collections.IList selectedItems) return;

        // Copy the items safely to a separate list before modifying the collection
        List<ListBoxItem> itemsToDelete = selectedItems.Cast<ListBoxItem>().ToList();

        foreach (ListBoxItem item in itemsToDelete)
        {
            FilesCollection.Remove(item);
        }
    }
}