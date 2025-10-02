using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Utils;
using JetBrains.Annotations;

namespace AlterTools.BatchExport.Views.Base;

public class ViewModelBase : NotifyPropertyChanged, IConfigBase
{
    protected static string NoFiles => Resources.Resources.Const_VMBase_NoFiles;

    private RelayCommand _browseFolderCommand;
    private RelayCommand _deleteCommand;
    private RelayCommand _eraseCommand;
    private RelayCommand _helpCommand;
    private RelayCommand _loadCommand;
    private RelayCommand _loadListCommand;
    private RelayCommand _raiseEventCommand;
    private RelayCommand _saveListCommand;
    
    private string _folderPath;
    
    private bool _isViewEnabled = true;
    
    private ObservableCollection<ListBoxItem> _listBoxItems = [];
    
    private ListBoxItem _selectedItem;

    private string _viewName = "Navisworks";

    public ObservableCollection<ListBoxItem> ListBoxItems
    {
        get => _listBoxItems;
        protected set => SetProperty(ref _listBoxItems, value);
    }

    [UsedImplicitly]
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

    [UsedImplicitly]
    public RelayCommand LoadListCommand => _loadListCommand ??= new RelayCommand(_ => LoadList());
    
    [UsedImplicitly]
    public RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
    
    [UsedImplicitly]
    public RelayCommand SaveListCommand => _saveListCommand ??= new RelayCommand(_ => SaveList());

    [UsedImplicitly]
    public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(_ => DeleteSelectedItems());

    [UsedImplicitly]
    public RelayCommand EraseCommand => _eraseCommand ??= new RelayCommand(_ => Erase());

    [UsedImplicitly]
    public RelayCommand BrowseFolderCommand => _browseFolderCommand ??= new RelayCommand(_ => BrowseFolder());

    protected string HelpMessage { get; set; }

    [UsedImplicitly]
    public RelayCommand HelpCommand =>
        _helpCommand ??= new RelayCommand(_ => MessageBox.Show(HelpMessage,
            Resources.Resources.Base_Button_Help_Content));

    protected EventHandlerBase EventHandlerBase { get; set; }

    [UsedImplicitly]
    public RelayCommand RaiseEventCommand => _raiseEventCommand ??= new RelayCommand(_ => EventHandlerBase.Raise(this));

    [UsedImplicitly]
    public virtual RelayCommand RadioButtonCommand { get; }

    public virtual string[] Files => [.. _listBoxItems.Select(item => item.Content.ToString())];

    public string ViewName
    {
        get => _viewName;
        set => SetProperty(ref _viewName, value);
    }

    public string FolderPath
    {
        get => _folderPath;
        set => SetProperty(ref _folderPath, value);
    }

    protected virtual void LoadList()
    {
        using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        IEnumerable<string> files = File.ReadLines(openFileDialog.FileName).FilterRevitFiles();

        ListBoxItems = [.. files.Select(DefaultListBoxItem)];

        if (!ListBoxItems.Any())
        {
            MessageBox.Show(NoFiles);
        }

        FolderPath = Path.GetDirectoryName(openFileDialog.FileName);
    }

    protected virtual void Load()
    {
        using OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        HashSet<string> existingFiles = [.. Files];

        IEnumerable<string> files = openFileDialog.FileNames
            .Distinct()
            .Where(file => !existingFiles.Contains(file));

        foreach (string file in files)
        {
            ListBoxItems.Add(DefaultListBoxItem(file));
        }
    }

    protected virtual void SaveList()
    {
        SaveFileDialog saveFileDialog = DialogType.RevitList.SaveFileDialog();
        if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

        string fileName = saveFileDialog.FileName;
        File.Delete(fileName);
        File.WriteAllLines(fileName, Files);

        FolderPath = Path.GetDirectoryName(saveFileDialog.FileName);
    }

    protected virtual void DeleteSelectedItems()
    {
        ListBoxItems.Where(item => item.IsSelected)
            .ToList()
            .ForEach(item => ListBoxItems.Remove(item));
    }

    protected virtual void Erase() => ListBoxItems.Clear();

    private void BrowseFolder()
    {
        FolderBrowserDialog folderBrowserDialog = new() { SelectedPath = FolderPath };

        if (folderBrowserDialog.ShowDialog() is DialogResult.OK)
        {
            FolderPath = folderBrowserDialog.SelectedPath;
        }
    }

    protected static ListBoxItem DefaultListBoxItem(string content)
    {
        return new ListBoxItem
        {
            Content = content,
            Background = Brushes.White
        };
    }
}