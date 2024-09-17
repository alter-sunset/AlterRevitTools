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

namespace VLS.BatchExportNet.Views
{
    public class ViewModelBase() : INotifyPropertyChanged
    {
        private ObservableCollection<ListBoxItem> _listBoxItems = [];
        public ObservableCollection<ListBoxItem> ListBoxItems
        { get => _listBoxItems; }

        private ListBoxItem _selectedItems;
        public ListBoxItem SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value;
                OnPropertyChanged("SelectedItems");
            }
        }
        private bool _isViewEnabled = true;
        public bool IsViewEnabled
        {
            get => _isViewEnabled;
            set
            {
                _isViewEnabled = value;
                OnPropertyChanged("IsViewEnabled");
            }
        }

        private RelayCommand _loadListCommand;
        public virtual RelayCommand LoadListCommand
        {
            get
            {
                return _loadListCommand ??= new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = new()
                    {
                        Multiselect = true,
                        DefaultExt = ".txt",
                        Filter = "Текстовый файл (.txt)|*.txt"
                    };

                    DialogResult result = openFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        _listBoxItems.Clear();

                        IEnumerable listRVTFiles = File.ReadLines(openFileDialog.FileName);

                        foreach (string rVTFile in listRVTFiles)
                        {
                            ListBoxItem listBoxItem = new() { Content = rVTFile, Background = Brushes.White };
                            if (!_listBoxItems.Any(cont => cont.Content.ToString() == rVTFile)
                                    && rVTFile.EndsWith(".rvt"))
                            {
                                _listBoxItems.Add(listBoxItem);
                            }
                        }

                        if (_listBoxItems.Count.Equals(0))
                        {
                            System.Windows.MessageBox.Show("В текстовом файле не было найдено подходящей информации");
                        }
                    }
                });
            }
        }

        private RelayCommand _loadCommand;
        public RelayCommand LoadCommand
        {
            get
            {
                return _loadCommand ??= new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = new()
                    {
                        Multiselect = true,
                        DefaultExt = ".rvt",
                        Filter = "Revit Files (.rvt)|*.rvt"
                    };

                    DialogResult result = openFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        foreach (string file in openFileDialog.FileNames)
                        {
                            ListBoxItem listBoxItem = new() { Content = file, Background = Brushes.White };
                            if (!_listBoxItems.Any(cont => cont.Content.ToString() == file))
                                _listBoxItems.Add(listBoxItem);
                        }
                    }
                });
            }
        }

        private RelayCommand _saveListCommand;
        public virtual RelayCommand SaveListCommand
        {
            get
            {
                return _saveListCommand ??= new RelayCommand(obj =>
                {
                    SaveFileDialog saveFileDialog = new()
                    {
                        FileName = "ListOfRVTFiles",
                        DefaultExt = ".txt",
                        Filter = "Текстовый файл (.txt)|*.txt"
                    };

                    DialogResult result = saveFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        string fileName = saveFileDialog.FileName;
                        File.Delete(fileName);

                        foreach (string fileRVT in _listBoxItems.Select(cont => cont.Content.ToString()))
                        {
                            if (!File.Exists(fileName))
                            {
                                File.WriteAllText(fileName, fileRVT);
                            }
                            else
                            {
                                string toWrite = "\n" + fileRVT;
                                File.AppendAllText(fileName, toWrite);
                            }
                        }
                    }
                });
            }
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
        public RelayCommand EraseCommand
        {
            get
            {
                return _eraseCommand ??= new RelayCommand(obj =>
                {
                    _listBoxItems.Clear();
                });
            }
        }

        private string _folderPath;
        public string FolderPath
        {
            get => _folderPath;
            set
            {
                _folderPath = value;
                OnPropertyChanged("FolderPath");
            }
        }

        private RelayCommand _browseFolderCommand;
        public RelayCommand BrowseFolderCommand
        {
            get
            {
                return _browseFolderCommand ??= new RelayCommand(obj =>
                {
                    FolderBrowserDialog folderBrowserDialog = new() { SelectedPath = FolderPath };
                    DialogResult result = folderBrowserDialog.ShowDialog();
                    string folderPath = folderBrowserDialog.SelectedPath;

                    if (result == DialogResult.OK)
                    {
                        FolderPath = folderPath;
                    }
                });
            }
        }
        public virtual RelayCommand HelpCommand { get; }
        public virtual RelayCommand RaiseEventCommand { get; }

        private void DeleteSelectedItems(object parameter)
        {
            List<ListBoxItem> selectedItems = ListBoxItems.Where(e => e.IsSelected).ToList();
            foreach (ListBoxItem item in selectedItems)
            {
                _listBoxItems.Remove(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
