using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using AlterTools.Resources;
using AlterTools.Utils.Extensions;
using AlterTools.Utils.MVVM;
using Autodesk.Revit.DB;

namespace AlterTools.atLink;

public class ViewModelMain : NotifyPropertyChanged, IConfigLink
{
    public static readonly ImportPlacement[] ImportPlacements =
    [
        ImportPlacement.Origin,
        ImportPlacement.Shared
    ];

    public Workset[] Worksets { get; }

    private readonly ExternalEventHandler _handler;

    private bool _pinLinks = true;
    private bool _isCurrentWorkset;

    private string _worksetPrefix = string.Empty;

    private ObservableCollection<Entry> _entries = [];
    private Entry _selectedEntry;

    public ViewModelMain(ExternalEventHandler eventHandler, Workset[] worksets)
    {
        Worksets = worksets;
        _handler = eventHandler;
    }

    public bool IsCurrentWorkset
    {
        get => _isCurrentWorkset;
        set => SetProperty(ref _isCurrentWorkset, value);
    }

    public bool PinLinks
    {
        get => _pinLinks;
        set => SetProperty(ref _pinLinks, value);
    }

    private string _folderPath;

    public string FolderPath
    {
        get => _folderPath;
        set => SetProperty(ref _folderPath, value);
    }


    public string[] Files => [.. Entries.Select(e => e.Name)];

    public ObservableCollection<Entry> Entries
    {
        get => _entries;
        set => SetProperty(ref _entries, value);
    }

    public Entry SelectedEntry
    {
        get => _selectedEntry;
        set => SetProperty(ref _selectedEntry, value);
    }

    public string WorksetPrefix
    {
        get => _worksetPrefix;
        set => SetProperty(ref _worksetPrefix, value);
    }

    public string[] WorksetPrefixes => _worksetPrefix.SplitBySemicolon();

    public void UpdateSelectedEntries(Entry sourceEntry, bool isWorkset)
    {
        foreach (Entry entry in Entries.Where(en => en != sourceEntry && en.IsSelected))
        {
            if (isWorkset)
            {
                entry.SelectedWorkset = sourceEntry.SelectedWorkset;
                continue;
            }

            entry.SelectedImportPlacement = sourceEntry.SelectedImportPlacement;
        }
    }

    private void LoadList()
    {
        using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        IEnumerable<string> files = File.ReadLines(openFileDialog.FileName).FilterRevitFiles();

        Entries = [.. files.Select(file => new Entry(this, file))];

        if (!Entries.Any())
        {
            MessageBox.Show(Strings.NoFiles);
        }

        FolderPath = Path.GetDirectoryName(openFileDialog.FileName);
    }

    private void Load()
    {
        using OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        HashSet<string> existingFiles = [.. Files];

        openFileDialog.FileNames.Where(file => !existingFiles.Contains(file))
            .Distinct()
            .Select(file => new Entry(this, file))
            .ToList()
            .ForEach(Entries.Add);
    }

    private void SaveList()
    {
        using SaveFileDialog saveFileDialog = DialogType.RevitList.SaveFileDialog();

        if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

        string fileName = saveFileDialog.FileName;
        File.Delete(fileName);
        File.WriteAllLines(fileName, Files);

        FolderPath = Path.GetDirectoryName(saveFileDialog.FileName);
    }

    private void DeleteSelectedItems()
    {
        Entries.Where(entry => entry.IsSelected)
            .ToList()
            .ForEach(entry => Entries.Remove(entry));
    }

    private void Erase() => Entries.Clear();

    private RelayCommand _executeCommand;
    public RelayCommand ExecuteCommand => _executeCommand ??= new RelayCommand(_ => Execute());

    private void Execute()
    {
        if (!ValidateInputs()) return;

        _handler.Raise(this);
    }

    private bool ValidateInputs()
    {
        if (Entries.Any()) return true;

        MessageBox.Show(Strings.ProvideFilesError);
        return false;
    }

    private bool _isViewEnabled = true;

    private string _viewName = "Navisworks";

    private RelayCommand _browseFolderCommand;
    private RelayCommand _deleteCommand;
    private RelayCommand _eraseCommand;
    private RelayCommand _helpCommand;
    private RelayCommand _loadCommand;
    private RelayCommand _loadListCommand;
    private RelayCommand _raiseEventCommand;
    private RelayCommand _saveListCommand;

    private ObservableCollection<ListBoxItem> _listBoxItems = [];
    private ListBoxItem _selectedItem;

    public ObservableCollection<ListBoxItem> ListBoxItems
    {
        get => _listBoxItems;
        protected set => SetProperty(ref _listBoxItems, value);
    }

    public ListBoxItem SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public bool IsViewEnabled
    {
        get => _isViewEnabled;
        set => SetProperty(ref _isViewEnabled, value);
    }

    public RelayCommand LoadListCommand => _loadListCommand ??= new RelayCommand(_ => LoadList());
    public RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
    public RelayCommand SaveListCommand => _saveListCommand ??= new RelayCommand(_ => SaveList());
    public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(_ => DeleteSelectedItems());
    public RelayCommand EraseCommand => _eraseCommand ??= new RelayCommand(_ => Erase());
    public RelayCommand BrowseFolderCommand => _browseFolderCommand ??= new RelayCommand(_ => BrowseFolder());

    private void BrowseFolder()
    {
        using FolderBrowserDialog folderBrowserDialog = new();
        folderBrowserDialog.SelectedPath = FolderPath;

        if (folderBrowserDialog.ShowDialog() is not DialogResult.OK) return;

        FolderPath = folderBrowserDialog.SelectedPath;
    }
}