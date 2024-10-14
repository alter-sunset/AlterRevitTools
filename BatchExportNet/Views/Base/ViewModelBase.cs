using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Base
{
    public class ViewModelBase() : INotifyPropertyChanged, IConfigBase
    {
        private ObservableCollection<ListBoxItem> _listBoxItems = [];
        public virtual ObservableCollection<ListBoxItem> ListBoxItems
        {
            get => _listBoxItems;
            set => SetProperty(ref _listBoxItems, value);
        }

        public List<string> Files
        {
            get => _listBoxItems.Select(e => e.Content.ToString()).ToList();
        }

        private ListBoxItem _selectedItems;
        public ListBoxItem SelectedItems
        {
            get => _selectedItems;
            set => SetProperty(ref _selectedItems, value);
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
            OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            ListBoxItems.Clear();

            IEnumerable listRVTFiles = File.ReadLines(openFileDialog.FileName);
            foreach (string rVTFile in listRVTFiles)
            {
                if (!ListBoxItems.Any(cont => cont.Content.ToString() == rVTFile)
                        && rVTFile.EndsWith(".rvt"))
                    ListBoxItems.Add(new() { Content = rVTFile, Background = Brushes.White });
            }
            if (!ListBoxItems.Any())
                MessageBox.Show("В текстовом файле не было найдено подходящей информации");

            FolderPath = Path.GetDirectoryName(openFileDialog.FileName);
        }

        private RelayCommand _loadCommand;
        public RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
        private void Load()
        {
            OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            foreach (string file in openFileDialog.FileNames)
            {
                if (!ListBoxItems.Any(cont => cont.Content.ToString() == file))
                    ListBoxItems.Add(new() { Content = file, Background = Brushes.White });
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
            File.WriteAllLines(fileName, ListBoxItems.Select(cont => cont.Content.ToString()));

            FolderPath = Path.GetDirectoryName(saveFileDialog.FileName);
        }

        private RelayCommand _deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new RelayCommand(DeleteSelectedItems);
            }
        }

        private RelayCommand _eraseCommand;
        public RelayCommand EraseCommand => _eraseCommand ??= new RelayCommand(obj => ListBoxItems.Clear());

        private string _folderPath;
        public string FolderPath
        {
            get => _folderPath;
            set => SetProperty(ref _folderPath, value);
        }

        private RelayCommand _browseFolderCommand;
        public virtual RelayCommand BrowseFolderCommand => _browseFolderCommand ??=
            new RelayCommand(obj => BrowseFolder());
        private void BrowseFolder()
        {
            FolderBrowserDialog folderBrowserDialog = new() { SelectedPath = FolderPath };
            DialogResult result = folderBrowserDialog.ShowDialog();
            string folderPath = folderBrowserDialog.SelectedPath;

            if (result is DialogResult.OK) FolderPath = folderPath;
        }
        private string _helpMessage;
        public string HelpMessage
        {
            get => _helpMessage;
            set => _helpMessage = value;
        }
        private RelayCommand _helpCommand;
        public virtual RelayCommand HelpCommand => _helpCommand ??=
            new RelayCommand(obj => MessageBox.Show(HelpMessage, "Справка"));

        private EventHandlerBase _eventHandlerBase;
        public EventHandlerBase EventHandlerBase
        {
            get => _eventHandlerBase;
            set => _eventHandlerBase = value;
        }
        private RelayCommand _raiseEventCommand;
        public RelayCommand RaiseEventCommand => _raiseEventCommand ??=
            new RelayCommand(obj => _eventHandlerBase.Raise(this));
        public virtual RelayCommand RadioButtonCommand { get; }

        private void DeleteSelectedItems(object parameter)
        {
            List<ListBoxItem> selectedItems = ListBoxItems.Where(e => e.IsSelected).ToList();
            foreach (ListBoxItem item in selectedItems)
            {
                ListBoxItems.Remove(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}