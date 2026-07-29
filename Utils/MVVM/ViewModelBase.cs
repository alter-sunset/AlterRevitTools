// using System.Collections.ObjectModel;
// using System.IO;
// using System.Windows.Controls;
// using AlterTools.Utils.Extensions;
// using AlterTools.Utils.Interfaces;
// using JetBrains.Annotations;
// using Brushes = System.Windows.Media.Brushes;
//
// namespace AlterTools.Utils.MVVM;
//
// public class ViewModelBase : NotifyPropertyChanged, IConfigBase
// {
//     internal protected static string NoFiles => Resources.Strings.NoFilesVMBase;
//
//     private bool _isViewEnabled = true;
//
//     private string _viewName = "Navisworks";
//     private string _folderPath;
//
//     private RelayCommand _browseFolderCommand;
//     private RelayCommand _deleteCommand;
//     private RelayCommand _eraseCommand;
//     private RelayCommand _helpCommand;
//     private RelayCommand _loadCommand;
//     private RelayCommand _loadListCommand;
//     private RelayCommand _raiseEventCommand;
//     private RelayCommand _saveListCommand;
//
//     private ObservableCollection<ListBoxItem> _listBoxItems = [];
//     private ListBoxItem _selectedItem;
//
//     public ObservableCollection<ListBoxItem> ListBoxItems
//     {
//         get => _listBoxItems;
//         protected set => SetProperty(ref _listBoxItems, value);
//     }
//
//     [UsedImplicitly]
//     public ListBoxItem SelectedItem
//     {
//         get => _selectedItem;
//         set => SetProperty(ref _selectedItem, value);
//     }
//
//     [UsedImplicitly]
//     public bool IsViewEnabled
//     {
//         get => _isViewEnabled;
//         set => SetProperty(ref _isViewEnabled, value);
//     }
//
//     [UsedImplicitly] public RelayCommand LoadListCommand => _loadListCommand ??= new RelayCommand(_ => LoadList());
//
//     [UsedImplicitly] public RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
//
//     [UsedImplicitly] public RelayCommand SaveListCommand => _saveListCommand ??= new RelayCommand(_ => SaveList());
//
//     [UsedImplicitly]
//     public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(_ => DeleteSelectedItems());
//
//     [UsedImplicitly] public RelayCommand EraseCommand => _eraseCommand ??= new RelayCommand(_ => Erase());
//
//     [UsedImplicitly]
//     public RelayCommand BrowseFolderCommand => _browseFolderCommand ??= new RelayCommand(_ => BrowseFolder());
//
//     internal protected string HelpMessage { get; set; }
//
//     [UsedImplicitly]
//     public RelayCommand HelpCommand =>
//         _helpCommand ??= new RelayCommand(_ => MessageBox.Show(HelpMessage, Resources.Strings.Help));
//
//     internal protected EventHandlerBase EventHandlerBase { get; set; }
//
//     [UsedImplicitly]
//     public RelayCommand RaiseEventCommand => _raiseEventCommand ??= new RelayCommand(_ => EventHandlerBase.Raise(this));
//
//     [UsedImplicitly] public virtual RelayCommand RadioButtonCommand { get; }
//
//     public virtual string[] Files => [.. _listBoxItems.Select(item => item.Content.ToString())];
//
//     public string ViewName
//     {
//         get => _viewName;
//         set => SetProperty(ref _viewName, value);
//     }
//
//     public string FolderPath
//     {
//         get => _folderPath;
//         set => SetProperty(ref _folderPath, value);
//     }
//
//     internal protected virtual void LoadList()
//     {
//         using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();
//
//         if (openFileDialog.ShowDialog() is not DialogResult.OK) return;
//
//         IEnumerable<string> files = File.ReadLines(openFileDialog.FileName).FilterRevitFiles();
//
//         ListBoxItems = [.. files.Select(DefaultListBoxItem)];
//
//         if (!ListBoxItems.Any())
//         {
//             MessageBox.Show(NoFiles);
//         }
//
//         FolderPath = Path.GetDirectoryName(openFileDialog.FileName);
//     }
//
//     internal protected virtual void Load()
//     {
//         using OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();
//
//         if (openFileDialog.ShowDialog() is not DialogResult.OK) return;
//
//         HashSet<string> existingFiles = [.. Files];
//
//         IEnumerable<string> files = openFileDialog.FileNames
//             .Distinct()
//             .Where(file => !existingFiles.Contains(file));
//
//         foreach (string file in files)
//         {
//             ListBoxItems.Add(DefaultListBoxItem(file));
//         }
//     }
//
//     internal protected virtual void SaveList()
//     {
//         using SaveFileDialog saveFileDialog = DialogType.RevitList.SaveFileDialog();
//         if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;
//
//         string fileName = saveFileDialog.FileName;
//         File.Delete(fileName);
//         File.WriteAllLines(fileName, Files);
//
//         FolderPath = Path.GetDirectoryName(saveFileDialog.FileName);
//     }
//
//     internal protected virtual void DeleteSelectedItems()
//     {
//         ListBoxItems.Where(item => item.IsSelected)
//             .ToList()
//             .ForEach(item => ListBoxItems.Remove(item));
//     }
//
//     internal protected virtual void Erase() => ListBoxItems.Clear();
//
//     private void BrowseFolder()
//     {
//         using FolderBrowserDialog folderBrowserDialog = new();
//         folderBrowserDialog.SelectedPath = FolderPath;
//
//         if (folderBrowserDialog.ShowDialog() is not DialogResult.OK) return;
//
//         FolderPath = folderBrowserDialog.SelectedPath;
//     }
//
//     internal protected static ListBoxItem DefaultListBoxItem(string content)
//     {
//         return new ListBoxItem
//         {
//             Content = content,
//             Background = Brushes.White
//         };
//     }
// }

