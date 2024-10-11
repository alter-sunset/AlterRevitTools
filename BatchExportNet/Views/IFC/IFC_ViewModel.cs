using Autodesk.Revit.DB;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.IFC
{
    public class IFC_ViewModel : ViewModelBase_Extended, IConfigIFC
    {
        public IFC_ViewModel(EventHandlerIFCExportVMArg eventHandlerIFCExportUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerIFCExportUiArg;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessageType.IFCTitle,
                    HelpMessageType.Load,
                    HelpMessageType.Folder,
                    HelpMessageType.Naming,
                    HelpMessageType.Config,
                    HelpMessageType.Start);
        }

        private string _mapping = "";
        public string Mapping
        {
            get => _mapping;
            set => SetProperty(ref _mapping, value);
        }
        public string FamilyMappingFile => _mapping;

        private RelayCommand _loadMappingCommand;
        public RelayCommand LoadMappingCommand => _loadMappingCommand ??= new RelayCommand(obj => LoadMapping());
        private void LoadMapping()
        {
            OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();
            if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

            try
            {
                Mapping = openFileDialog.FileName;
            }
            catch
            {
                MessageBox.Show("Неверная схема файла");
            }
        }

        private bool _exportBaseQuantities = false;
        public bool ExportBaseQuantities
        {
            get => _exportBaseQuantities;
            set => SetProperty(ref _exportBaseQuantities, value);
        }

        private bool _wallAndColumnSplitting = false;
        public bool WallAndColumnSplitting
        {
            get => _wallAndColumnSplitting;
            set => SetProperty(ref _wallAndColumnSplitting, value);
        }

        private RelayCommand _loadListCommand;
        public override RelayCommand LoadListCommand => _loadListCommand ??= new RelayCommand(obj => LoadList());
        private void LoadList()
        {
            OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();
            if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

            using FileStream file = File.OpenRead(openFileDialog.FileName);
            IFCFormDeserilaizer(JsonHelper<IFCForm>.DeserializeConfig(file));
        }
        private void IFCFormDeserilaizer(IFCForm form)
        {
            if (form is null) return;

            FolderPath = form.FolderPath;
            NamePrefix = form.NamePrefix;
            NamePostfix = form.NamePostfix;
            WorksetPrefix = string.Join(';', form.WorksetPrefixes);
            Mapping = form.FamilyMappingFile;
            ExportBaseQuantities = form.ExportBaseQuantities;
            SelectedVersion = _ifcVersions.FirstOrDefault(e => e.Key == form.FileVersion);
            WallAndColumnSplitting = form.WallAndColumnSplitting;
            ExportScopeView = form.ExportView;
            ViewName = form.ViewName;
            SelectedLevel = _spaceBoundaryLevels.FirstOrDefault(e => e.Key == form.SpaceBoundaryLevel);
            ListBoxItems.Clear();
            foreach (string file in form.Files)
            {
                if (string.IsNullOrEmpty(file)) continue;

                if (!ListBoxItems.Any(cont => cont.Content.ToString() == file)
                    || file.EndsWith(".rvt", true, System.Globalization.CultureInfo.CurrentCulture))
                    ListBoxItems.Add(new() { Content = file, Background = Brushes.White });
            }
        }

        private RelayCommand _saveListCommand;
        public override RelayCommand SaveListCommand => _saveListCommand ??= new RelayCommand(obj => SaveList());
        private void SaveList()
        {
            using IFCForm form = IFCFormSerializer();
            SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

            if (saveFileDialog.ShowDialog() is not DialogResult.OK)
            {
                form.Dispose();
                return;
            }

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

            Files = ListBoxItems
                .Select(cont => cont.Content.ToString() ?? string.Empty)
                .ToList()
        };

        private readonly IReadOnlyDictionary<IFCVersion, string> _ifcVersions
            = IFC_Context.IFCVersions;
        public IReadOnlyDictionary<IFCVersion, string> IFCVersions => _ifcVersions;

        private KeyValuePair<IFCVersion, string> _selectedVersion
            = IFC_Context.IFCVersions.FirstOrDefault(e => e.Key == IFCVersion.Default);
        public KeyValuePair<IFCVersion, string> SelectedVersion
        {
            get => _selectedVersion;
            set => SetProperty(ref _selectedVersion, value);
        }
        public IFCVersion FileVersion => _selectedVersion.Key;

        private readonly IReadOnlyDictionary<int, string> _spaceBoundaryLevels
            = IFC_Context.SpaceBoundaryLevels;
        public IReadOnlyDictionary<int, string> SpaceBoundaryLevels => _spaceBoundaryLevels;

        private KeyValuePair<int, string> _selectedLevel
            = IFC_Context.SpaceBoundaryLevels.FirstOrDefault(e => e.Key == 1);
        public KeyValuePair<int, string> SelectedLevel
        {
            get => _selectedLevel;
            set => SetProperty(ref _selectedLevel, value);
        }
        public int SpaceBoundaryLevel => SelectedLevel.Key;
    }
}