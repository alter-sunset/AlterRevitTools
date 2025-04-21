using Autodesk.Revit.DB;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.IFC
{
    public class IFCViewModel : ViewModelBaseExtended, IConfigIFC
    {
        public IFCViewModel(EventHandlerIFC eventHandlerIFC)
        {
            EventHandlerBase = eventHandlerIFC;
            HelpMessage = Help.GetHelpDictionary()
                              .GetResultMessage(HelpMessageType.IFCTitle,
                                                HelpMessageType.Load,
                                                HelpMessageType.Folder,
                                                HelpMessageType.Naming,
                                                HelpMessageType.Config,
                                                HelpMessageType.Start);
        }

        private string _mapping = string.Empty;
        public string Mapping
        {
            get => _mapping;
            set => SetProperty(ref _mapping, value);
        }
        public string FamilyMappingFile => _mapping;

        private RelayCommand _loadMappingCommand;
        public RelayCommand LoadMappingCommand => _loadMappingCommand ??= new RelayCommand(_ => LoadMapping());
        private void LoadMapping()
        {
            using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();

            if (DialogResult.OK != openFileDialog.ShowDialog()) return;

            try
            {
                Mapping = openFileDialog.FileName;
            }
            catch
            {
                MessageBox.Show("Неверная схема файла");
            }
        }

        private bool _exportBaseQuantities;
        public bool ExportBaseQuantities
        {
            get => _exportBaseQuantities;
            set => SetProperty(ref _exportBaseQuantities, value);
        }

        private bool _wallAndColumnSplitting;
        public bool WallAndColumnSplitting
        {
            get => _wallAndColumnSplitting;
            set => SetProperty(ref _wallAndColumnSplitting, value);
        }

        protected override void LoadList()
        {
            OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();

            if (DialogResult.OK != openFileDialog.ShowDialog()) return;

            using FileStream file = File.OpenRead(openFileDialog.FileName);

            IFCFormDeserializer(JsonHelper<IFCForm>.DeserializeConfig(file));
        }
        private void IFCFormDeserializer(IFCForm form)
        {
            if (form is null) return;

            FolderPath = form.FolderPath;
            NamePrefix = form.NamePrefix;
            NamePostfix = form.NamePostfix;
            WorksetPrefix = string.Join(";", form.WorksetPrefixes);
            Mapping = form.FamilyMappingFile;
            ExportBaseQuantities = form.ExportBaseQuantities;
            SelectedVersion = IFCVersions.FirstOrDefault(ver => ver.Key == form.FileVersion);
            WallAndColumnSplitting = form.WallAndColumnSplitting;
            ExportScopeView = form.ExportView;
            ViewName = form.ViewName;
            SelectedLevel = SpaceBoundaryLevels.FirstOrDefault(level => level.Key == form.SpaceBoundaryLevel);

            IEnumerable<string> files = form.Files.FilterRevitFiles();

            ListBoxItems = new ObservableCollection<ListBoxItem>(files.Select(DefaultListBoxItem));
        }

        protected override void SaveList()
        {
            using IFCForm form = IFCFormSerializer();

            SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

            if (DialogResult.OK != saveFileDialog.ShowDialog()) return;

            string fileName = saveFileDialog.FileName;
            File.Delete(fileName);

            JsonHelper<IFCForm>.SerializeConfig(form, fileName);
        }
        private IFCForm IFCFormSerializer() => new()
        {
            ExportBaseQuantities = ExportBaseQuantities,
            FamilyMappingFile = Mapping,
            FileVersion = SelectedVersion.Key,
            SpaceBoundaryLevel = SelectedLevel.Key,
            WallAndColumnSplitting = WallAndColumnSplitting,
            FolderPath = FolderPath,
            NamePrefix = NamePrefix,
            NamePostfix = NamePostfix,
            WorksetPrefixes = WorksetPrefixes,
            ExportView = ExportScopeView,
            ViewName = ViewName,

            Files = ListBoxItems.Select(item => item.Content.ToString() ?? string.Empty)
                                .ToArray()
        };

        private KeyValuePair<IFCVersion, string> _selectedVersion = IFCContext.IFCVersions.FirstOrDefault(ver => IFCVersion.Default == ver.Key);
        public IReadOnlyDictionary<IFCVersion, string> IFCVersions { get; } = IFCContext.IFCVersions;
        public KeyValuePair<IFCVersion, string> SelectedVersion
        {
            get => _selectedVersion;
            set => SetProperty(ref _selectedVersion, value);
        }
        public IFCVersion FileVersion => _selectedVersion.Key;

        private KeyValuePair<int, string> _selectedLevel = IFCContext.SpaceBoundaryLevels.FirstOrDefault(e => 1 == e.Key);
        public IReadOnlyDictionary<int, string> SpaceBoundaryLevels { get; } = IFCContext.SpaceBoundaryLevels;

        public KeyValuePair<int, string> SelectedLevel
        {
            get => _selectedLevel;
            set => SetProperty(ref _selectedLevel, value);
        }
        public int SpaceBoundaryLevel => SelectedLevel.Key;
    }
}