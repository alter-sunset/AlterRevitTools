using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.NWC
{
    public class NWCViewModel : ViewModelBaseExtended, IConfigNWC
    {
        private readonly EventHandlerNWCBatch _eventHandlerNWCBatch;

        private ObservableCollection<Config> _configs = [];

        private bool _convertElementProperties;

        private bool _convertLights;

        private bool _convertLinkedCADFormats;

        private bool _divideFileIntoLevels = true;

        private bool _exportElementIds = true;

        private bool _exportLinks;

        private bool _exportParts;

        private bool _exportRoomAsAttribute = true;

        private bool _exportRoomGeometry;

        private bool _exportUrls;

        private double _facetingFactor = 1;

        private bool _findMissingMaterials;

        private RelayCommand _loadConfigsCommand;

        private RelayCommand _raiseBatchEventCommand;

        private KeyValuePair<NavisworksCoordinates, string> _selectedCoordinates =
            NWCContext.Coordinates.FirstOrDefault(coord => coord.Key == NavisworksCoordinates.Shared);

        private KeyValuePair<NavisworksParameters, string> _selectedParameters =
            NWCContext.Parameters.FirstOrDefault(param => param.Key is NavisworksParameters.All);

        public NWCViewModel(EventHandlerNWCBatch eventHandlerNWCBatch, EventHandlerNWC eventHandlerNWC)
        {
            _eventHandlerNWCBatch = eventHandlerNWCBatch;
            EventHandlerBase = eventHandlerNWC;
            HelpMessage = Help.GetHelpDictionary()
                .GetResultMessage(HelpMessageType.NWCTitle,
                    HelpMessageType.Load,
                    HelpMessageType.Folder,
                    HelpMessageType.Naming,
                    HelpMessageType.Config,
                    HelpMessageType.Start,
                    HelpMessageType.NWCEnd);
        }

        public IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates { get; } = NWCContext.Coordinates;

        public KeyValuePair<NavisworksCoordinates, string> SelectedCoordinates
        {
            get => _selectedCoordinates;
            set => SetProperty(ref _selectedCoordinates, value);
        }

        public IReadOnlyDictionary<NavisworksParameters, string> Parameters { get; } = NWCContext.Parameters;

        public KeyValuePair<NavisworksParameters, string> SelectedParameters
        {
            get => _selectedParameters;
            set => SetProperty(ref _selectedParameters, value);
        }

        public ObservableCollection<Config> Configs
        {
            get => _configs;
            set => SetProperty(ref _configs, value);
        }

        public RelayCommand LoadConfigsCommand => _loadConfigsCommand ??= new RelayCommand(_ => LoadConfig());

        public RelayCommand RaiseBatchEventCommand =>
            _raiseBatchEventCommand ??= new RelayCommand(_ => _eventHandlerNWCBatch.Raise(this));

        public bool ConvertElementProperties
        {
            get => _convertElementProperties;
            set => SetProperty(ref _convertElementProperties, value);
        }

        public bool DivideFileIntoLevels
        {
            get => _divideFileIntoLevels;
            set => SetProperty(ref _divideFileIntoLevels, value);
        }

        public bool ExportElementIds
        {
            get => _exportElementIds;
            set => SetProperty(ref _exportElementIds, value);
        }

        public bool ExportLinks
        {
            get => _exportLinks;
            set => SetProperty(ref _exportLinks, value);
        }

        public bool ExportParts
        {
            get => _exportParts;
            set => SetProperty(ref _exportParts, value);
        }

        public bool ExportRoomAsAttribute
        {
            get => _exportRoomAsAttribute;
            set => SetProperty(ref _exportRoomAsAttribute, value);
        }

        public bool ExportRoomGeometry
        {
            get => _exportRoomGeometry;
            set => SetProperty(ref _exportRoomGeometry, value);
        }

        public bool ExportUrls
        {
            get => _exportUrls;
            set => SetProperty(ref _exportUrls, value);
        }

        public bool FindMissingMaterials
        {
            get => _findMissingMaterials;
            set => SetProperty(ref _findMissingMaterials, value);
        }

        public bool ConvertLinkedCADFormats
        {
            get => _convertLinkedCADFormats;
            set => SetProperty(ref _convertLinkedCADFormats, value);
        }

        public bool ConvertLights
        {
            get => _convertLights;
            set => SetProperty(ref _convertLights, value);
        }

        public double FacetingFactor
        {
            get => _facetingFactor;
            set => SetProperty(ref _facetingFactor, value);
        }

        NavisworksParameters IConfigNWC.Parameters => _selectedParameters.Key;
        NavisworksCoordinates IConfigNWC.Coordinates => _selectedCoordinates.Key;

        protected override void LoadList()
        {
            OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();

            if (DialogResult.OK != openFileDialog.ShowDialog())
            {
                return;
            }

            using FileStream file = File.OpenRead(openFileDialog.FileName);

            NWCFormDeserilaizer(JsonHelper<NWCForm>.DeserializeConfig(file));
        }

        public void NWCFormDeserilaizer(NWCForm form)
        {
            if (null == form)
            {
                return;
            }

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
            FolderPath = form.FolderPath;
            NamePrefix = form.NamePrefix;
            NamePostfix = form.NamePostfix;
            WorksetPrefix = string.Join(";", form.WorksetPrefixes);
            ExportScopeView = NavisworksExportScope.View == form.ExportScope;
            ListBoxItems = new ObservableCollection<ListBoxItem>(form.Files
                .FilterRevitFiles()
                .Select(DefaultListBoxItem));
            ConvertLights = form.ConvertLights;
            ConvertLinkedCADFormats = form.ConvertLinkedCADFormats;
            FacetingFactor = form.FacetingFactor;
            SelectedCoordinates = Coordinates.FirstOrDefault(coord => coord.Key == form.Coordinates);
            SelectedParameters = Parameters.FirstOrDefault(param => param.Key == form.Parameters);
        }

        protected override void SaveList()
        {
            using NWCForm form = NWCFormSerializer();
            using SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

            if (DialogResult.OK != saveFileDialog.ShowDialog())
            {
                return;
            }

            string fileName = saveFileDialog.FileName;

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            JsonHelper<NWCForm>.SerializeConfig(form, fileName);
        }

        private NWCForm NWCFormSerializer()
        {
            return new NWCForm
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
                FolderPath = FolderPath,
                NamePrefix = NamePrefix,
                NamePostfix = NamePostfix,
                WorksetPrefixes = WorksetPrefix.Split(';')
                    .Select(prefix => prefix.Trim())
                    .ToArray(),
                ConvertLights = ConvertLights,
                ConvertLinkedCADFormats = ConvertLinkedCADFormats,
                FacetingFactor = FacetingFactor,
                Files = ListBoxItems.Select(item => item.Content.ToString() ?? string.Empty)
                    .ToArray()
            };
        }

        private void LoadConfig()
        {
            using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();

            if (DialogResult.OK != openFileDialog.ShowDialog())
            {
                return;
            }

            IEnumerable<string> configs = File.ReadLines(openFileDialog.FileName);

            Configs = new ObservableCollection<Config>(configs
                .Where(config => config.EndsWith(".json") && File.Exists(config))
                .Select(config => new Config(config)));

            if (!Configs.Any())
            {
                MessageBox.Show(NO_FILES);
            }
        }

        protected override void DeleteSelectedItems()
        {
            ListBoxItems.Where(item => item.IsSelected)
                .ToList()
                .ForEach(item => ListBoxItems.Remove(item));

            Configs.Where(config => config.IsSelected)
                .ToList()
                .ForEach(config => Configs.Remove(config));
        }
    }
}