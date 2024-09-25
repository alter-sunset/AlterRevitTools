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
    public class IFC_ViewModel : ViewModelBase_Extended
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
            set
            {
                _mapping = value.Trim();
                OnPropertyChanged(nameof(Mapping));
            }
        }

        private RelayCommand _loadMapping;
        public RelayCommand LoadMapping
        {
            get
            {
                return _loadMapping ??= new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();

                    DialogResult result = openFileDialog.ShowDialog();

                    if (result != DialogResult.OK)
                        return;

                    try
                    {
                        Mapping = openFileDialog.FileName;
                    }
                    catch
                    {
                        MessageBox.Show(AlertType.WrongConfig.GetAlert());
                    }
                });
            }
        }

        private bool _exportBaseQuantities = false;
        public bool ExportBaseQuantities
        {
            get => _exportBaseQuantities;
            set
            {
                _exportBaseQuantities = value;
                OnPropertyChanged(nameof(ExportBaseQuantities));
            }
        }

        private bool _wallAndColumnSplitting = false;
        public bool WallAndColumnSplitting
        {
            get => _wallAndColumnSplitting;
            set
            {
                _wallAndColumnSplitting = value;
                OnPropertyChanged(nameof(WallAndColumnSplitting));
            }
        }

        private RelayCommand _loadListCommand;
        public override RelayCommand LoadListCommand
        {
            get
            {
                return _loadListCommand ??= new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();

                    DialogResult result = openFileDialog.ShowDialog();

                    if (result != DialogResult.OK)
                        return;

                    using FileStream file = File.OpenRead(openFileDialog.FileName);
                    IFCFormDeserilaizer(JsonHelper<IFCForm>.DeserializeConfig(file));
                });
            }
        }
        private void IFCFormDeserilaizer(IFCForm form)
        {
            if (form is null)
                return;

            FolderPath = form.DestinationFolder;
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
            foreach (string file in form.RVTFiles)
            {
                if (string.IsNullOrEmpty(file))
                    continue;

                ListBoxItem listBoxItem = new() { Content = file, Background = Brushes.White };
                if (!ListBoxItems.Any(cont => cont.Content.ToString() == file)
                    || file.EndsWith(".rvt", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    ListBoxItems.Add(listBoxItem);
                }
            }
        }

        private RelayCommand _saveListCommand;
        public override RelayCommand SaveListCommand
        {
            get
            {
                return _saveListCommand ??= new RelayCommand(obj =>
                {
                    using IFCForm form = IFCFormSerializer();
                    SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();
                    DialogResult result = saveFileDialog.ShowDialog();

                    if (result != DialogResult.OK)
                    {
                        form.Dispose();
                        return;
                    }

                    string fileName = saveFileDialog.FileName;
                    File.Delete(fileName);

                    JsonHelper<IFCForm>.SerializeConfig(form, fileName);
                });
            }
        }
        private IFCForm IFCFormSerializer() => new()
        {
            ExportBaseQuantities = ExportBaseQuantities,
            FamilyMappingFile = Mapping,
            FileVersion = SelectedVersion.Key,
            SpaceBoundaryLevel = SelectedLevel.Key,
            WallAndColumnSplitting = WallAndColumnSplitting,
            DestinationFolder = FolderPath,
            NamePrefix = NamePrefix,
            NamePostfix = NamePostfix,
            WorksetPrefixes = WorksetPrefix
                .Split(';')
                .Select(e => e.Trim())
                .ToArray(),
            ExportView = ExportScopeView,
            ViewName = ViewName,

            RVTFiles = ListBoxItems
                .Select(cont => cont.Content.ToString())
                .ToList()
        };

        private readonly Dictionary<IFCVersion, string> _ifcVersions
            = IFC_Context.IFCVersions;
        public Dictionary<IFCVersion, string> IFCVersions
        {
            get => _ifcVersions;
        }
        private KeyValuePair<IFCVersion, string> _selectedVersion
            = IFC_Context.IFCVersions.FirstOrDefault(e => e.Key == IFCVersion.Default);
        public KeyValuePair<IFCVersion, string> SelectedVersion
        {
            get => _selectedVersion;
            set
            {
                _selectedVersion = value;
                OnPropertyChanged(nameof(SelectedVersion));
            }
        }

        private readonly Dictionary<int, string> _spaceBoundaryLevels
            = IFC_Context.SpaceBoundaryLevels;
        public Dictionary<int, string> SpaceBoundaryLevels
        {
            get => _spaceBoundaryLevels;
        }
        private KeyValuePair<int, string> _selectedLevel
            = IFC_Context.SpaceBoundaryLevels.FirstOrDefault(e => e.Key == 1);
        public KeyValuePair<int, string> SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                _selectedLevel = value;
                OnPropertyChanged(nameof(SelectedLevel));
            }
        }
    }
}