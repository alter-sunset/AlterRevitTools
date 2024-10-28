using Autodesk.Revit.DB;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Link
{
    public class LinkViewModel : ViewModelBase
    {
        public LinkViewModel(EventHandlerLink eventHandlerLink)
        {
            EventHandlerBase = eventHandlerLink;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessageType.LinkTitle,
                    HelpMessageType.Load,
                    HelpMessageType.List,
                    HelpMessageType.Start);
        }

        private bool _isCurrentWorkset = true;
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
        public void UpdateSelectedEntries(Entry sourceEntry)
        {
            foreach (Entry entry in Entries.Where(e => e != sourceEntry && e.IsSelected))
            {
                entry.SelectedImportPlacement = sourceEntry.SelectedImportPlacement;
            }
        }
        private RelayCommand _loadListCommand;
        public override RelayCommand LoadListCommand => _loadListCommand ??= new RelayCommand(_ => LoadList());
        private void LoadList()
        {
            using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();
            if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

            IEnumerable<string> files = File.ReadLines(openFileDialog.FileName)
                .Distinct()
                .Where(f => !string.IsNullOrWhiteSpace(f) &&
                    f.EndsWith(".rvt", StringComparison.OrdinalIgnoreCase));

            Entries = new ObservableCollection<Entry>(files.Select(e => new Entry(this, e)));

            if (!Entries.Any())
                MessageBox.Show(NO_FILES);

            FolderPath = Path.GetDirectoryName(openFileDialog.FileName);
        }

        private RelayCommand _loadCommand;
        public override RelayCommand LoadCommand => _loadCommand ??= new RelayCommand(_ => Load());
        private void Load()
        {
            using OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();
            if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

            HashSet<string> existingFiles = new(Files);

            IEnumerable<Entry> files = openFileDialog.FileNames
                .Where(f => !existingFiles.Contains(f))
                .Distinct()
                .Select(f => new Entry(this, f));

            foreach (Entry file in files)
            {
                Entries.Add(file);
            }
        }

        private RelayCommand _saveListCommand;
        public override RelayCommand SaveListCommand => _saveListCommand ??= new RelayCommand(_ => SaveList());
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
        public override RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(DeleteSelectedItems);
        private void DeleteSelectedItems(object parameter)
        {
            Entry[] selectedItems = Entries.Where(e => e.IsSelected).ToArray();
            foreach (Entry item in selectedItems)
            {
                Entries.Remove(item);
            }
        }

        private RelayCommand _eraseCommand;
        public override RelayCommand EraseCommand => _eraseCommand ??= new RelayCommand(obj => Entries.Clear());
    }
}