using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Utils;

namespace AlterTools.BatchExport.Views.Base
{
    public class ViewModelBase : NotifyPropertyChanged, IConfigBase
    {
        protected const string NO_FILES = "В текстовом файле не было найдено подходящей информации";

        private ObservableCollection<ListBoxItem> _listBoxItems = [];

        public ObservableCollection<ListBoxItem> ListBoxItems
        {
            get => _listBoxItems;
            protected set => SetProperty(ref _listBoxItems, value);
        }

        public virtual string[] Files => _listBoxItems.Select(item => item.Content.ToString()).ToArray();

        private ListBoxItem _selectedItem;
        public ListBoxItem SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }
        private bool _isViewEnabled = true;
        public bool IsViewEnabled
        {
            get => _isViewEnabled;
            set => SetProperty(ref _isViewEnabled, value);
        }

        private string _viewName = "Navisworks";
        public string ViewName
        {
            get => _viewName;
            set => SetProperty(ref _viewName, value);
        }

        private RelayCommand _loadListCommand;
        public RelayCommand LoadListCommand => _loadListCommand ??= new RelayCommand(_ => LoadList());
        protected virtual void LoadList()
        {
            using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();

            if (DialogResult.OK != openFileDialog.ShowDialog()) return;

            IEnumerable<string> files = File.ReadLines(openFileDialog.FileName).FilterRevitFiles();

            ListBoxItems = new ObservableCollection<ListBoxItem>(files.Select(DefaultListBoxItem));

            if (!ListBoxItems.Any())
            {
                MessageBox.Show(NO_FILES);
            }

            FolderPath = Path.GetDirectoryName(openFileDialog.FileName);
        }

        private RelayCommand _loadCommand;
        public RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
        protected virtual void Load()
        {
            using OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();

            if (DialogResult.OK != openFileDialog.ShowDialog()) return;

            HashSet<string> existingFiles = new(Files);

            IEnumerable<string> files = openFileDialog.FileNames.Distinct()
                                                                .Where(file => !existingFiles.Contains(file));

            foreach (string file in files)
            {
                ListBoxItems.Add(DefaultListBoxItem(file));
            }
        }

        private RelayCommand _saveListCommand;
        public RelayCommand SaveListCommand => _saveListCommand ??= new RelayCommand(_ => SaveList());
        protected virtual void SaveList()
        {
            SaveFileDialog saveFileDialog = DialogType.RevitList.SaveFileDialog();
            if (DialogResult.OK != saveFileDialog.ShowDialog()) return;

            string fileName = saveFileDialog.FileName;
            File.Delete(fileName);
            File.WriteAllLines(fileName, Files);

            FolderPath = Path.GetDirectoryName(saveFileDialog.FileName);
        }

        private RelayCommand _deleteCommand;
        public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(_ => DeleteSelectedItems());
        protected virtual void DeleteSelectedItems()
        {
            ListBoxItems.Where(item => item.IsSelected)
                        .ToList()
                        .ForEach(item => ListBoxItems.Remove(item));
        }

        private RelayCommand _eraseCommand;
        public RelayCommand EraseCommand => _eraseCommand ??= new RelayCommand(_ => Erase());

        protected virtual void Erase() => ListBoxItems.Clear();

        private string _folderPath;
        public string FolderPath
        {
            get => _folderPath;
            set => SetProperty(ref _folderPath, value);
        }

        private RelayCommand _browseFolderCommand;
        public RelayCommand BrowseFolderCommand => _browseFolderCommand ??= new RelayCommand(_ => BrowseFolder());
        private void BrowseFolder()
        {
            FolderBrowserDialog folderBrowserDialog = new() { SelectedPath = FolderPath };

            if (DialogResult.OK == folderBrowserDialog.ShowDialog())
            {
                FolderPath = folderBrowserDialog.SelectedPath;
            }
        }

        protected string HelpMessage { get; set; }

        private RelayCommand _helpCommand;
        public RelayCommand HelpCommand => _helpCommand ??= new RelayCommand(_ => MessageBox.Show(HelpMessage, "Справка"));

        protected EventHandlerBase EventHandlerBase { get; set; }

        private RelayCommand _raiseEventCommand;
        public RelayCommand RaiseEventCommand => _raiseEventCommand ??= new RelayCommand(_ => EventHandlerBase.Raise(this));
        public virtual RelayCommand RadioButtonCommand { get; }

        protected static ListBoxItem DefaultListBoxItem(string content) => new()
        {
            Content = content,
            Background = Brushes.White
        };
    }
}