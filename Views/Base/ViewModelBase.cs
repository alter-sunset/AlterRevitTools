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
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Views.Base
{
    public class ViewModelBase() : INotifyPropertyChanged
    {
        private ObservableCollection<ListBoxItem> _listBoxItems = [];
        public virtual ObservableCollection<ListBoxItem> ListBoxItems
        {
            get => _listBoxItems;
            set
            {
                _listBoxItems = value;
                OnPropertyChanged(nameof(ListBoxItems));
            }
        }

        private ListBoxItem _selectedItems;
        public ListBoxItem SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value;
                OnPropertyChanged(nameof(SelectedItems));
            }
        }
        private bool _isViewEnabled = true;
        public bool IsViewEnabled
        {
            get => _isViewEnabled;
            set
            {
                _isViewEnabled = value;
                OnPropertyChanged(nameof(IsViewEnabled));
            }
        }

        private string _viewName = "Navisworks";
        public string ViewName
        {
            get => _viewName;
            set
            {
                _viewName = value.Trim();
                OnPropertyChanged(nameof(ViewName));
            }
        }

        private RelayCommand _loadListCommand;
        public virtual RelayCommand LoadListCommand
        {
            get
            {
                return _loadListCommand ??= new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();
                    DialogResult result = openFileDialog.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    ListBoxItems.Clear();

                    IEnumerable listRVTFiles = File.ReadLines(openFileDialog.FileName);
                    foreach (string rVTFile in listRVTFiles)
                    {
                        ListBoxItem listBoxItem = new() { Content = rVTFile, Background = Brushes.White };
                        if (!ListBoxItems.Any(cont => cont.Content.ToString() == rVTFile)
                                && rVTFile.EndsWith(".rvt"))
                        {
                            ListBoxItems.Add(listBoxItem);
                        }
                    }
                    if (ListBoxItems.Count.Equals(0))
                    {
                        MessageBox.Show(AlertType.EmptyList.GetAlert());
                    }
                    FolderPath = Path.GetDirectoryName(openFileDialog.FileName);
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
                    OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();

                    DialogResult result = openFileDialog.ShowDialog();

                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    foreach (string file in openFileDialog.FileNames)
                    {
                        ListBoxItem listBoxItem = new() { Content = file, Background = Brushes.White };
                        if (!ListBoxItems.Any(cont => cont.Content.ToString() == file))
                            ListBoxItems.Add(listBoxItem);
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
                    SaveFileDialog saveFileDialog = DialogType.RevitList.SaveFileDialog();

                    DialogResult result = saveFileDialog.ShowDialog();

                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    string fileName = saveFileDialog.FileName;
                    File.Delete(fileName);

                    foreach (string fileRVT in ListBoxItems.Select(cont => cont.Content.ToString()))
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
                    FolderPath = Path.GetDirectoryName(saveFileDialog.FileName);
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
                    ListBoxItems.Clear();
                });
            }
        }

        private string _folderPath;
        public string FolderPath
        {
            get => _folderPath;
            set
            {
                _folderPath = value.Trim();
                OnPropertyChanged(nameof(FolderPath));
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
        private string _helpMessage;
        public string HelpMessage
        {
            get => _helpMessage;
            set => _helpMessage = value;
        }
        private RelayCommand _helpCommand;
        public virtual RelayCommand HelpCommand
        {
            get
            {
                return _helpCommand ??= new RelayCommand(obj =>
                {
                    MessageBox.Show(HelpMessage, "Справка");
                });
            }
        }

        private EventHandlerBaseVMArgs _eventHandlerBaseVMArgs;
        public EventHandlerBaseVMArgs EventHandlerBaseVMArgs
        {
            get => _eventHandlerBaseVMArgs;
            set => _eventHandlerBaseVMArgs = value;
        }
        private RelayCommand _raiseEventCommand;
        public RelayCommand RaiseEventCommand
        {
            get
            {
                return _raiseEventCommand ??= new RelayCommand(obj =>
                {
                    _eventHandlerBaseVMArgs.Raise(this);
                });
            }
        }
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
    }
}