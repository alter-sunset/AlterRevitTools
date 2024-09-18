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
using VLS.BatchExportNet.Source;

namespace VLS.BatchExportNet.Views.NWC
{
    public class NWC_ViewModel : ViewModelBase_Extended
    {
        private readonly EventHandlerNWCExportBatchUiArg _eventHandlerNWCExportBatchUiArg;
        private readonly EventHandlerNWCExportUiArg _eventHandlerNWCExportUiArg;
        private const string HELP_MESSAGE = "\tПлагин предназначен для пакетного экспорта файлов в формат NWC." +
                 "\n" +
                 "\tЕсли вы впервые используете плагин, и у вас нет ранее сохранённых файлов конфигурации, то вам необходимо выполнить следующее: " +
                 "используя кнопку \"Загрузить\" добавьте все модели объекта, которые необходимо экспортировать. " +
                 "Если случайно были добавлены лишние файлы, выделите их и нажмите кнопку \"Удалить\"" +
                 "\n" +
                 "\tДалее укажите папку для сохранения. Прописать путь можно в ручную или же выбрать папку используя кнопку \"Обзор\"." +
                 "\n" +
                 "\tЗадайте префикс и постфикс, которые будет необходимо добавить в название файлов. Если такой необходимости нет, то оставьте поля пустыми." +
                 "\n" +
                 "\tВыберите необходимые свойства экспорта. По умолчанию стоят стандартные настройки, с которыми чаще всего работают." +
                 "\n" +
                 "\tСохраните конфигурацию кнопкой \"Сохранить список\" в формате (.JSON)." +
                 "\n" +
                 "\tДалее эту конфигурацию можно будет использовать для повторного экспорта, используя кнопку \"Загрузить список\"." +
                 "\n\n" +
                 "\tЗапустите экспорт кнопкой \"ОК\"." +
                 "\n\n" +
                 "**********************************************" +
                 "\n\n" +
                 "\tЕсли у вас есть несколько сохранённых конфигураций, то можно использовать пакетный экспорт второго уровня." +
                 "\n" +
                 "\tКнопкой \"Загрузить конфиги\" загрузите список (.txt) с путями к конфигурациям в формате (.JSON). " +
                 "Структура списка выглядит следующим образом: \n\tpath\\\\config.json\n\tpath\\\\config2.json\n\tpath\\\\config3.json" +
                 "\n" +
                 "\tКнопкой \"Начать\" запустите пакетный экспорт второго уровня, который экспортирует несколько объектов с соответствующими им настройками.";
        public NWC_ViewModel(EventHandlerNWCExportBatchUiArg eventHandlerNWCExportBatchUiArg,
                EventHandlerNWCExportUiArg eventHandlerNWCExportUiArg)
        {
            _eventHandlerNWCExportBatchUiArg = eventHandlerNWCExportBatchUiArg;
            _eventHandlerNWCExportUiArg = eventHandlerNWCExportUiArg;
            HelpMessage = HELP_MESSAGE;
        }

        private bool _convertElementProperties = false;
        public bool ConvertElementProperties
        {
            get => _convertElementProperties;
            set
            {
                _convertElementProperties = value;
                OnPropertyChanged("ConvertElementProperties");
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
                OnPropertyChanged("SelectedCoordinates");
            }
        }

        private bool _divideFileIntoLevels = true;
        public bool DivideFileIntoLevels
        {
            get => _divideFileIntoLevels;
            set
            {
                _divideFileIntoLevels = value;
                OnPropertyChanged("DivideFileIntoLevels");
            }
        }

        private bool _exportElementIds = true;
        public bool ExportElementIds
        {
            get => _exportElementIds;
            set
            {
                _exportElementIds = value;
                OnPropertyChanged("ExportElementIds");
            }
        }

        private bool _exportLinks = false;
        public bool ExportLinks
        {
            get => _exportLinks;
            set
            {
                _exportLinks = value;
                OnPropertyChanged("ExportLinks");
            }
        }

        private bool _exportParts = false;
        public bool ExportParts
        {
            get => _exportParts;
            set
            {
                _exportParts = value;
                OnPropertyChanged("ExportParts");
            }
        }

        private bool _exportRoomAsAttribute = true;
        public bool ExportRoomAsAttribute
        {
            get => _exportRoomAsAttribute;
            set
            {
                _exportRoomAsAttribute = value;
                OnPropertyChanged("ExportRoomAsAttribute");
            }
        }

        private bool _exportRoomGeometry = false;
        public bool ExportRoomGeometry
        {
            get => _exportRoomGeometry;
            set
            {
                _exportRoomGeometry = value;
                OnPropertyChanged("ExportRoomGeometry");
            }
        }

        private bool _exportUrls = false;
        public bool ExportUrls
        {
            get => _exportUrls;
            set
            {
                _exportUrls = value;
                OnPropertyChanged("ExportUrls");
            }
        }

        private bool _findMissingMaterials = false;
        public bool FindMissingMaterials
        {
            get => _findMissingMaterials;
            set
            {
                _findMissingMaterials = value;
                OnPropertyChanged("FindMissingMaterials");
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
                OnPropertyChanged("SelectedParameters");
            }
        }

        private bool _convertLinkedCADFormats = false;
        public bool ConvertLinkedCADFormats
        {
            get => _convertLinkedCADFormats;
            set
            {
                _convertLinkedCADFormats = value;
                OnPropertyChanged("ConvertLinkedCADFormats");
            }
        }

        private bool _convertLights = false;
        public bool ConvertLights
        {
            get => _convertLights;
            set
            {
                _convertLights = value;
                OnPropertyChanged("ConvertLights");
            }
        }

        private string _facetingFactor = "1";
        public string FacetingFactor
        {
            get => _facetingFactor;
            set
            {
                _facetingFactor = value;
                OnPropertyChanged("FacetingFactor");
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

                    if (result == DialogResult.OK)
                    {
                        using FileStream file = File.OpenRead(openFileDialog.FileName);
                        try
                        {
                            JsonSerializerOptions options = new()
                            {
                                WriteIndented = true
                            };
                            NWCForm form = JsonSerializer.Deserialize<NWCForm>(file, options);
                            NWCFormDeserilaizer(form);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Неверная схема файла\n{ex.Message}");
                        }
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
            WorksetPrefix = form.WorksetPrefix;
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
                    if (result == DialogResult.OK)
                    {
                        string fileName = saveFileDialog.FileName;
                        File.Delete(fileName);
                        try
                        {
                            JsonSerializerOptions options = new()
                            {
                                WriteIndented = true
                            };
                            File.WriteAllText(fileName, JsonSerializer.Serialize(form, options));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Неверная схема файла\n{ex.Message}");
                        }
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
            WorksetPrefix = WorksetPrefix,
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
                OnPropertyChanged("Configs");
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

                    if (result == DialogResult.OK)
                    {
                        IEnumerable<string> configs = File.ReadLines(openFileDialog.FileName);
                        Configs = new ObservableCollection<string>(
                            configs.Where(e => !Configs.Any(c => c == e) && e.EndsWith(".json")));

                        if (Configs.Count.Equals(0))
                        {
                            MessageBox.Show("В текстовом файле не было найдено подходящей информации");
                        }
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

        private RelayCommand _raiseEventCommand;
        public override RelayCommand RaiseEventCommand
        {
            get
            {
                return _raiseEventCommand ??= new RelayCommand(obj =>
                {
                    _eventHandlerNWCExportUiArg.Raise(this);
                });
            }
        }
    }
}