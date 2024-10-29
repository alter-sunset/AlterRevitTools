using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Base
{
    public class ViewModelBase() : NotifyPropertyChanged, IConfigBase
    {
        public const string NO_FILES = "В текстовом файле не было найдено подходящей информации";
        private ObservableCollection<ListBoxItem> _listBoxItems = [];
        public virtual ObservableCollection<ListBoxItem> ListBoxItems
        {
            get => _listBoxItems;
            set => SetProperty(ref _listBoxItems, value);
        }

        public virtual string[] Files => _listBoxItems.Select(e => e.Content.ToString()).ToArray();

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
        public virtual RelayCommand LoadListCommand => _loadListCommand ??= new RelayCommand(_ => LoadList());
        private void LoadList()
        {
            using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();
            if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

            IEnumerable<string> files = File.ReadLines(openFileDialog.FileName)
                .Distinct()
                .Where(f => !string.IsNullOrWhiteSpace(f) &&
                    f.EndsWith(".rvt", StringComparison.OrdinalIgnoreCase));

            ListBoxItems = new ObservableCollection<ListBoxItem>(files.Select(DefaultListBoxItem));

            if (!ListBoxItems.Any())
                MessageBox.Show(NO_FILES);

            FolderPath = Path.GetDirectoryName(openFileDialog.FileName);
        }

        private RelayCommand _loadCommand;
        public virtual RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
        private void Load()
        {
            using OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();
            if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

            HashSet<string> existingFiles = new(Files);

            IEnumerable<string> files = openFileDialog.FileNames
                .Distinct()
                .Where(f => !existingFiles.Contains(f));

            foreach (string file in files)
            {
                ListBoxItems.Add(DefaultListBoxItem(file));
            }
        }

        private RelayCommand _saveListCommand;
        public virtual RelayCommand SaveListCommand => _saveListCommand ??= new RelayCommand(_ => SaveList());
        private void SaveList()
        {
            SaveFileDialog saveFileDialog = DialogType.RevitList.SaveFileDialog();
            if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

            string fileName = saveFileDialog.FileName;
            File.Delete(fileName);
            File.WriteAllLines(fileName, Files);

            FolderPath = Path.GetDirectoryName(saveFileDialog.FileName);
        }

        private RelayCommand _deleteCommand;
        public virtual RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(DeleteSelectedItems);
        private void DeleteSelectedItems(object parameter) => ListBoxItems
            .Where(e => e.IsSelected)
            .ToList()
            .ForEach(item => ListBoxItems.Remove(item));

        private RelayCommand _eraseCommand;
        public virtual RelayCommand EraseCommand => _eraseCommand ??= new RelayCommand(obj => ListBoxItems.Clear());

        private string _folderPath;
        public string FolderPath
        {
            get => _folderPath;
            set => SetProperty(ref _folderPath, value);
        }

        private RelayCommand _browseFolderCommand;
        public virtual RelayCommand BrowseFolderCommand => _browseFolderCommand ??= new RelayCommand(obj => BrowseFolder());
        private void BrowseFolder()
        {
            FolderBrowserDialog folderBrowserDialog = new() { SelectedPath = FolderPath };
            DialogResult result = folderBrowserDialog.ShowDialog();
            string folderPath = folderBrowserDialog.SelectedPath;

            if (result is DialogResult.OK)
                FolderPath = folderPath;
        }
        private string _helpMessage;
        public string HelpMessage
        {
            get => _helpMessage;
            set => _helpMessage = value;
        }
        private RelayCommand _helpCommand;
        public virtual RelayCommand HelpCommand => _helpCommand ??= new RelayCommand(obj => MessageBox.Show(HelpMessage, "Справка"));

        private EventHandlerBase _eventHandlerBase;
        public EventHandlerBase EventHandlerBase
        {
            get => _eventHandlerBase;
            set => _eventHandlerBase = value;
        }
        private RelayCommand _raiseEventCommand;
        public RelayCommand RaiseEventCommand => _raiseEventCommand ??= new RelayCommand(obj => _eventHandlerBase.Raise(this));
        public virtual RelayCommand RadioButtonCommand { get; }

        public static ListBoxItem DefaultListBoxItem(string content) =>
            new()
            {
                Content = content,
                Background = Brushes.White
            };
    }
}