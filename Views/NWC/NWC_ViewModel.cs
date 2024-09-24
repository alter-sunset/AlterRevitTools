using Autodesk.Revit.DB;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Views.NWC
{
    public class NWC_ViewModel : ViewModelBase_Extended
    {
        private readonly EventHandlerNWCExportBatchVMArg _eventHandlerNWCExportBatchUiArg;
        public NWC_ViewModel(EventHandlerNWCExportBatchVMArg eventHandlerNWCExportBatchUiArg,
                EventHandlerNWCExportVMArg eventHandlerNWCExportUiArg)
        {
            _eventHandlerNWCExportBatchUiArg = eventHandlerNWCExportBatchUiArg;
            EventHandlerBaseVMArgs = eventHandlerNWCExportUiArg;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessages.NWCTitle,
                    HelpMessages.Load,
                    HelpMessages.Folder,
                    HelpMessages.Naming,
                    HelpMessages.Config,
                    HelpMessages.Start,
                    HelpMessages.NWCEnd);
        }

        private bool _convertElementProperties = false;
        public bool ConvertElementProperties
        {
            get => _convertElementProperties;
            set
            {
                _convertElementProperties = value;
                OnPropertyChanged(nameof(ConvertElementProperties));
            }
        }

        private readonly Dictionary<NavisworksCoordinates, string> _coordinates
            = NWC_Context.Coordinates;
        public Dictionary<NavisworksCoordinates, string> Coordinates
        {
            get => _coordinates;
        }
        private KeyValuePair<NavisworksCoordinates, string> _selectedCoordinates
            = NWC_Context.Coordinates.FirstOrDefault(e => e.Key == NavisworksCoordinates.Shared);
        public KeyValuePair<NavisworksCoordinates, string> SelectedCoordinates
        {
            get => _selectedCoordinates;
            set
            {
                _selectedCoordinates = value;
                OnPropertyChanged(nameof(SelectedCoordinates));
            }
        }

        private bool _divideFileIntoLevels = true;
        public bool DivideFileIntoLevels
        {
            get => _divideFileIntoLevels;
            set
            {
                _divideFileIntoLevels = value;
                OnPropertyChanged(nameof(DivideFileIntoLevels));
            }
        }

        private bool _exportElementIds = true;
        public bool ExportElementIds
        {
            get => _exportElementIds;
            set
            {
                _exportElementIds = value;
                OnPropertyChanged(nameof(ExportElementIds));
            }
        }

        private bool _exportLinks = false;
        public bool ExportLinks
        {
            get => _exportLinks;
            set
            {
                _exportLinks = value;
                OnPropertyChanged(nameof(ExportLinks));
            }
        }

        private bool _exportParts = false;
        public bool ExportParts
        {
            get => _exportParts;
            set
            {
                _exportParts = value;
                OnPropertyChanged(nameof(ExportParts));
            }
        }

        private bool _exportRoomAsAttribute = true;
        public bool ExportRoomAsAttribute
        {
            get => _exportRoomAsAttribute;
            set
            {
                _exportRoomAsAttribute = value;
                OnPropertyChanged(nameof(ExportRoomAsAttribute));
            }
        }

        private bool _exportRoomGeometry = false;
        public bool ExportRoomGeometry
        {
            get => _exportRoomGeometry;
            set
            {
                _exportRoomGeometry = value;
                OnPropertyChanged(nameof(ExportRoomGeometry));
            }
        }

        private bool _exportUrls = false;
        public bool ExportUrls
        {
            get => _exportUrls;
            set
            {
                _exportUrls = value;
                OnPropertyChanged(nameof(ExportUrls));
            }
        }

        private bool _findMissingMaterials = false;
        public bool FindMissingMaterials
        {
            get => _findMissingMaterials;
            set
            {
                _findMissingMaterials = value;
                OnPropertyChanged(nameof(FindMissingMaterials));
            }
        }

        private readonly Dictionary<NavisworksParameters, string> _parameters
            = NWC_Context.Parameters;
        public Dictionary<NavisworksParameters, string> Parameters
        {
            get => _parameters;
        }
        private KeyValuePair<NavisworksParameters, string> _selectedParameters
            = NWC_Context.Parameters.FirstOrDefault(e => e.Key == NavisworksParameters.All);
        public KeyValuePair<NavisworksParameters, string> SelectedParameters
        {
            get => _selectedParameters;
            set
            {
                _selectedParameters = value;
                OnPropertyChanged(nameof(SelectedParameters));
            }
        }

        private bool _convertLinkedCADFormats = false;
        public bool ConvertLinkedCADFormats
        {
            get => _convertLinkedCADFormats;
            set
            {
                _convertLinkedCADFormats = value;
                OnPropertyChanged(nameof(ConvertLinkedCADFormats));
            }
        }

        private bool _convertLights = false;
        public bool ConvertLights
        {
            get => _convertLights;
            set
            {
                _convertLights = value;
                OnPropertyChanged(nameof(ConvertLights));
            }
        }

        private string _facetingFactor = "1";
        public string FacetingFactor
        {
            get => _facetingFactor;
            set
            {
                _facetingFactor = value.Trim();
                OnPropertyChanged(nameof(FacetingFactor));
            }
        }

        private RelayCommand _loadListCommand;
        public override RelayCommand LoadListCommand
        {
            get
            {
                return _loadListCommand ??= new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = new()
                    {
                        Multiselect = false,
                        DefaultExt = ".json",
                        Filter = "Файл JSON (.json)|*.json"
                    };

                    DialogResult result = openFileDialog.ShowDialog();

                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    using FileStream file = File.OpenRead(openFileDialog.FileName);
                    try
                    {
                        JsonSerializerOptions options = JsonHelper.GetDefaultOptions();
                        NWCForm form = JsonSerializer.Deserialize<NWCForm>(file, options);
                        NWCFormDeserilaizer(form);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Неверная схема файла\n{ex.Message}");
                    }
                });
            }
        }
        public void NWCFormDeserilaizer(NWCForm form)
        {
            ConvertElementProperties = form.ConvertElementProperties;
            DivideFileIntoLevels = form.DivideFileIntoLevels;
            ExportElementIds = form.ExportElementIds;
            ExportLinks = form.ExportLinks;
            ExportParts = form.ExportParts;
            ExportRoomAsAttribute = form.ExportRoomAsAttribute;
            ExportRoomGeometry = form.ExportRoomGeometry;
            ExportUrls = form.ExportUrls;
            FindMissingMaterials = form.FindMissingMaterials;
            ViewName = form.ViewName;
            FolderPath = form.DestinationFolder;
            NamePrefix = form.NamePrefix;
            NamePostfix = form.NamePostfix;
            WorksetPrefix = string.Join(';', form.WorksetPrefixes);
            ExportScopeView = form.ExportScope == NavisworksExportScope.View;
            ListBoxItems.Clear();
            foreach (string file in form.RVTFiles)
            {
                if (string.IsNullOrEmpty(file))
                {
                    continue;
                }

                ListBoxItem listBoxItem = new() { Content = file, Background = Brushes.White };
                if (!ListBoxItems.Any(cont => cont.Content.ToString() == file)
                    || file.EndsWith(".rvt", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    ListBoxItems.Add(listBoxItem);
                }
            }
            ConvertLights = form.ConvertLights;
            ConvertLinkedCADFormats = form.ConvertLinkedCADFormats;
            FacetingFactor = form.FacetingFactor.ToString();
            SelectedCoordinates = _coordinates.FirstOrDefault(e => e.Key == form.Coordinates);
            SelectedParameters = _parameters.FirstOrDefault(e => e.Key == form.Parameters);
        }

        private RelayCommand _saveListCommand;
        public override RelayCommand SaveListCommand
        {
            get
            {
                return _saveListCommand ??= new RelayCommand(obj =>
                {
                    NWCForm form = NWCFormSerializer();
                    SaveFileDialog saveFileDialog = new()
                    {
                        FileName = "ConfigBatchExportNWC",
                        DefaultExt = ".json",
                        Filter = "Файл JSON (.json)|*.json"
                    };
                    DialogResult result = saveFileDialog.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        form.Dispose();
                        return;
                    }

                    string fileName = saveFileDialog.FileName;
                    File.Delete(fileName);
                    try
                    {
                        JsonSerializerOptions options = JsonHelper.GetDefaultOptions();
                        File.WriteAllText(fileName, JsonSerializer.Serialize(form, options));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Неверная схема файла\n{ex.Message}");
                    }
                    form.Dispose();
                });
            }
        }
        private NWCForm NWCFormSerializer() => new()
        {
            ConvertElementProperties = ConvertElementProperties,
            DivideFileIntoLevels = DivideFileIntoLevels,
            ExportElementIds = ExportElementIds,
            ExportLinks = ExportLinks,
            ExportParts = ExportParts,
            ExportRoomAsAttribute = ExportRoomAsAttribute,
            ExportRoomGeometry = ExportRoomGeometry,
            ExportUrls = ExportUrls,
            FindMissingMaterials = FindMissingMaterials,
            Coordinates = SelectedCoordinates.Key,
            Parameters = SelectedParameters.Key,
            ExportScope = ExportScopeView ? NavisworksExportScope.View : NavisworksExportScope.Model,
            ViewName = ViewName,
            DestinationFolder = FolderPath,
            NamePrefix = NamePrefix,
            NamePostfix = NamePostfix,
            WorksetPrefixes = WorksetPrefix
                .Split(';')
                .Select(e => e.Trim())
                .ToArray(),
            ConvertLights = ConvertLights,
            ConvertLinkedCADFormats = ConvertLinkedCADFormats,
            FacetingFactor = double.TryParse(FacetingFactor, out double facetingFactor) ? facetingFactor : 1.0,

            RVTFiles = ListBoxItems
                .Select(cont => cont.Content.ToString())
                .ToList()
        };

        private ObservableCollection<string> _configs = [];
        public ObservableCollection<string> Configs
        {
            get => _configs;
            set
            {
                _configs = value;
                OnPropertyChanged(nameof(Configs));
            }
        }

        private RelayCommand _loadConfigsCommand;
        public RelayCommand LoadConfigsCommand
        {
            get
            {
                return _loadConfigsCommand ??= new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = new()
                    {
                        Multiselect = false,
                        DefaultExt = ".txt",
                        Filter = "Текстовый файл (.txt)|*.txt"
                    };

                    DialogResult result = openFileDialog.ShowDialog();

                    if (result != DialogResult.OK)
                    {
                        return;
                    }

                    IEnumerable<string> configs = File.ReadLines(openFileDialog.FileName);
                    Configs = new ObservableCollection<string>(
                        configs.Where(e => !Configs.Any(c => c == e) && e.EndsWith(".json")));

                    if (Configs.Count.Equals(0))
                    {
                        MessageBox.Show("В текстовом файле не было найдено подходящей информации");
                    }
                });
            }
        }

        private RelayCommand _raiseBatchEventCommand;
        public RelayCommand RaiseBatchEventCommand
        {
            get
            {
                return _raiseBatchEventCommand ??= new RelayCommand(obj =>
                {
                    _eventHandlerNWCExportBatchUiArg.Raise(this);
                });
            }
        }
    }
}