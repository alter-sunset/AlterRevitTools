using Autodesk.Revit.DB;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AlterTools.BatchExport.Source.EventHandlers;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Utils;

namespace AlterTools.BatchExport.Views.Link
{
    public class LinkViewModel : ViewModelBase
    {
        public LinkViewModel(EventHandlerLink eventHandlerLink, Workset[] worksets)
        {
            Worksets = worksets;
            EventHandlerBase = eventHandlerLink;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessageType.LinkTitle,
                    HelpMessageType.Load,
                    HelpMessageType.List,
                    HelpMessageType.Start);
        }

        private bool _isCurrentWorkset = false;
        public bool IsCurrentWorkset
        {
            get => _isCurrentWorkset;
            set => SetProperty(ref _isCurrentWorkset, value);
        }
        public override string[] Files => Entries.Select(e => e.Name).ToArray();

        public static readonly ImportPlacement[] ImportPlacements = new ImportPlacement[]
        {
            ImportPlacement.Origin,
            ImportPlacement.Shared
        };
        public readonly Workset[] Worksets;

        private ObservableCollection<Entry> _entries = new ObservableCollection<Entry>();
        public ObservableCollection<Entry> Entries
        {
            get => _entries;
            set => SetProperty(ref _entries, value);
        }

        private Entry _selectedEntry;
        public Entry SelectedEntry
        {
            get => _selectedEntry;
            set => SetProperty(ref _selectedEntry, value);
        }
        public void UpdateSelectedEntries(Entry sourceEntry, bool isWorkset)
        {
            foreach (Entry entry in Entries.Where(e => e != sourceEntry && e.IsSelected))
            {
                if (isWorkset)
                    entry.SelectedWorkset = sourceEntry.SelectedWorkset;
                else
                    entry.SelectedImportPlacement = sourceEntry.SelectedImportPlacement;
            }
        }
        private RelayCommand _loadListCommand;
        public override RelayCommand LoadListCommand
        {
            get
            {
                if (_loadListCommand is null)
                    _loadListCommand = new RelayCommand(obj => LoadList());
                return _loadListCommand;
            }
        }
        private void LoadList()
        {
            using (OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog())
            {
                if (!(openFileDialog.ShowDialog() is DialogResult.OK)) return;

                IEnumerable<string> files = File.ReadLines(openFileDialog.FileName).FilterRevitFiles();

                Entries = new ObservableCollection<Entry>(files.Select(e => new Entry(this, e)));

                if (!Entries.Any())
                    MessageBox.Show(NO_FILES);

                FolderPath = Path.GetDirectoryName(openFileDialog.FileName);
            }
        }

        private RelayCommand _loadCommand;
        public override RelayCommand LoadCommand
        {
            get
            {
                if (_loadCommand is null)
                    _loadCommand = new RelayCommand(obj => Load());
                return _loadCommand;
            }
        }
        private void Load()
        {
            using (OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog())
            {
                if (!(openFileDialog.ShowDialog() is DialogResult.OK)) return;

                HashSet<string> existingFiles = new HashSet<string>(Files);

                openFileDialog.FileNames
                    .Where(f => !existingFiles.Contains(f))
                    .Distinct()
                    .Select(f => new Entry(this, f))
                    .ToList()
                    .ForEach(Entries.Add);
            }
        }

        private RelayCommand _saveListCommand;
        public override RelayCommand SaveListCommand
        {
            get
            {
                if (_saveListCommand is null)
                    _saveListCommand = new RelayCommand(obj => SaveList());
                return _saveListCommand;
            }
        }
        private void SaveList()
        {
            SaveFileDialog saveFileDialog = DialogType.RevitList.SaveFileDialog();
            if (!(saveFileDialog.ShowDialog() is DialogResult.OK)) return;

            string fileName = saveFileDialog.FileName;
            File.Delete(fileName);
            File.WriteAllLines(fileName, Files);

            FolderPath = Path.GetDirectoryName(saveFileDialog.FileName);
        }

        private RelayCommand _deleteCommand;
        public override RelayCommand DeleteCommand
        {
            get
            {
                if (_deleteCommand is null)
                    _deleteCommand = new RelayCommand(obj => DeleteSelectedItems());
                return _deleteCommand;
            }
        }
        private void DeleteSelectedItems() => Entries
            .Where(e => e.IsSelected)
            .ToList()
            .ForEach(item => Entries.Remove(item));

        private RelayCommand _eraseCommand;
        public override RelayCommand EraseCommand
        {
            get
            {
                if (_eraseCommand is null)
                    _eraseCommand = new RelayCommand(obj => Entries.Clear());
                return _eraseCommand;
            }
        }
    }
}