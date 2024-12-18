using Autodesk.Revit.DB;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AlterTools.BatchExportNet.Source.EventHandlers;
using AlterTools.BatchExportNet.Views.Base;
using AlterTools.BatchExportNet.Utils;

namespace AlterTools.BatchExportNet.Views.Link
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

        public static readonly ImportPlacement[] ImportPlacements =
            [
                ImportPlacement.Origin,
                ImportPlacement.Shared
            ];
        public readonly Workset[] Worksets;

        private ObservableCollection<Entry> _entries = [];
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
        public override RelayCommand LoadListCommand =>
            _loadListCommand ??= new RelayCommand(obj => LoadList());
        private void LoadList()
        {
            using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();
            if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

            IEnumerable<string> files = File.ReadLines(openFileDialog.FileName).FilterRevitFiles();

            Entries = new ObservableCollection<Entry>(files.Select(e => new Entry(this, e)));

            if (!Entries.Any())
                MessageBox.Show(NO_FILES);

            FolderPath = Path.GetDirectoryName(openFileDialog.FileName);
        }

        private RelayCommand _loadCommand;
        public override RelayCommand LoadCommand =>
            _loadCommand ??= new RelayCommand(obj => Load());
        private void Load()
        {
            using OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();
            if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

            HashSet<string> existingFiles = new(Files);

            openFileDialog.FileNames
                .Where(f => !existingFiles.Contains(f))
                .Distinct()
                .Select(f => new Entry(this, f))
                .ToList()
                .ForEach(Entries.Add);
        }

        private RelayCommand _saveListCommand;
        public override RelayCommand SaveListCommand =>
            _saveListCommand ??= new RelayCommand(obj => SaveList());
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
        public override RelayCommand DeleteCommand =>
            _deleteCommand ??= new RelayCommand(obj => DeleteSelectedItems());
        private void DeleteSelectedItems() => Entries
            .Where(e => e.IsSelected)
            .ToList()
            .ForEach(item => Entries.Remove(item));

        private RelayCommand _eraseCommand;
        public override RelayCommand EraseCommand =>
            _eraseCommand ??= new RelayCommand(obj => Entries.Clear());
    }
}