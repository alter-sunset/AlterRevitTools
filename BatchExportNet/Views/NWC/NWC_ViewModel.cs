using Autodesk.Revit.DB;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Base;
using System;

namespace VLS.BatchExportNet.Views.NWC
{
    public class NWC_ViewModel : ViewModelBase_Extended, IConfigNWC
    {
        private readonly EventHandlerNWCExportBatchVMArg _eventHandlerNWCExportBatchUiArg;
        public NWC_ViewModel(EventHandlerNWCExportBatchVMArg eventHandlerNWCExportBatchUiArg,
                EventHandlerNWCExportVMArg eventHandlerNWCExportUiArg)
        {
            _eventHandlerNWCExportBatchUiArg = eventHandlerNWCExportBatchUiArg;
            EventHandlerBaseVMArgs = eventHandlerNWCExportUiArg;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessageType.NWCTitle,
                    HelpMessageType.Load,
                    HelpMessageType.Folder,
                    HelpMessageType.Naming,
                    HelpMessageType.Config,
                    HelpMessageType.Start,
                    HelpMessageType.NWCEnd);
        }

        private bool _convertElementProperties = false;
        public bool ConvertElementProperties
        {
            get => _convertElementProperties;
            set => SetProperty(ref _convertElementProperties, value);
        }

        private readonly IReadOnlyDictionary<NavisworksCoordinates, string> _coordinates
            = NWC_Context.Coordinates;
        public IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates => _coordinates;

        private KeyValuePair<NavisworksCoordinates, string> _selectedCoordinates
            = NWC_Context.Coordinates.FirstOrDefault(e => e.Key == NavisworksCoordinates.Shared);
        public KeyValuePair<NavisworksCoordinates, string> SelectedCoordinates
        {
            get => _selectedCoordinates;
            set => SetProperty(ref _selectedCoordinates, value);
        }

        private bool _divideFileIntoLevels = true;
        public bool DivideFileIntoLevels
        {
            get => _divideFileIntoLevels;
            set => SetProperty(ref _divideFileIntoLevels, value);
        }

        private bool _exportElementIds = true;
        public bool ExportElementIds
        {
            get => _exportElementIds;
            set => SetProperty(ref _exportElementIds, value);
        }

        private bool _exportLinks = false;
        public bool ExportLinks
        {
            get => _exportLinks;
            set => SetProperty(ref _exportLinks, value);
        }

        private bool _exportParts = false;
        public bool ExportParts
        {
            get => _exportParts;
            set => SetProperty(ref _exportParts, value);
        }

        private bool _exportRoomAsAttribute = true;
        public bool ExportRoomAsAttribute
        {
            get => _exportRoomAsAttribute;
            set => SetProperty(ref _exportRoomAsAttribute, value);
        }

        private bool _exportRoomGeometry = false;
        public bool ExportRoomGeometry
        {
            get => _exportRoomGeometry;
            set => SetProperty(ref _exportRoomGeometry, value);
        }

        private bool _exportUrls = false;
        public bool ExportUrls
        {
            get => _exportUrls;
            set => SetProperty(ref _exportUrls, value);
        }

        private bool _findMissingMaterials = false;
        public bool FindMissingMaterials
        {
            get => _findMissingMaterials;
            set => SetProperty(ref _findMissingMaterials, value);
        }

        private readonly IReadOnlyDictionary<NavisworksParameters, string> _parameters
            = NWC_Context.Parameters;
        public IReadOnlyDictionary<NavisworksParameters, string> Parameters => _parameters;

        private KeyValuePair<NavisworksParameters, string> _selectedParameters
            = NWC_Context.Parameters.FirstOrDefault(e => e.Key == NavisworksParameters.All);
        public KeyValuePair<NavisworksParameters, string> SelectedParameters
        {
            get => _selectedParameters;
            set => SetProperty(ref _selectedParameters, value);
        }

        private bool _convertLinkedCADFormats = false;
        public bool ConvertLinkedCADFormats
        {
            get => _convertLinkedCADFormats;
            set => SetProperty(ref _convertLinkedCADFormats, value);
        }

        private bool _convertLights = false;
        public bool ConvertLights
        {
            get => _convertLights;
            set => SetProperty(ref _convertLights, value);
        }

        private double _facetingFactor = 1;
        public double FacetingFactor
        {
            get => _facetingFactor;
            set => SetProperty(ref _facetingFactor, value);
        }

        private RelayCommand _loadListCommand;
        public override RelayCommand LoadListCommand => _loadListCommand ??= new RelayCommand(_ => LoadList());
        private void LoadList()
        {
            OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();
            if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

            using FileStream file = File.OpenRead(openFileDialog.FileName);
            NWCFormDeserilaizer(JsonHelper<NWCForm>.DeserializeConfig(file));
        }
        public void NWCFormDeserilaizer(NWCForm form)
        {
            if (form is null) return;

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
            WorksetPrefix = string.Join(';', form.WorksetPrefixes);
            ExportScopeView = form.ExportScope == NavisworksExportScope.View;
            ListBoxItems.Clear();
            foreach (string file in form.Files)
            {
                if (string.IsNullOrEmpty(file)) continue;

                if (!ListBoxItems.Any(cont => cont.Content.ToString() == file)
                    || file.EndsWith(".rvt", StringComparison.OrdinalIgnoreCase))
                {
                    ListBoxItems.Add(new ListBoxItem { Content = file, Background = Brushes.White });
                }
            }
            ConvertLights = form.ConvertLights;
            ConvertLinkedCADFormats = form.ConvertLinkedCADFormats;
            FacetingFactor = form.FacetingFactor;
            SelectedCoordinates = _coordinates.FirstOrDefault(e => e.Key == form.Coordinates);
            SelectedParameters = _parameters.FirstOrDefault(e => e.Key == form.Parameters);
        }

        private RelayCommand _saveListCommand;
        public override RelayCommand SaveListCommand => _saveListCommand ??= new RelayCommand(_ => SaveList());
        private void SaveList()
        {
            using NWCForm form = NWCFormSerializer();
            using SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();
            if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

            string fileName = saveFileDialog.FileName;
            if (File.Exists(fileName)) File.Delete(fileName);

            JsonHelper<NWCForm>.SerializeConfig(form, fileName);
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
            FolderPath = FolderPath,
            NamePrefix = NamePrefix,
            NamePostfix = NamePostfix,
            WorksetPrefixes = WorksetPrefix
                .Split(';')
                .Select(e => e.Trim())
                .ToArray(),
            ConvertLights = ConvertLights,
            ConvertLinkedCADFormats = ConvertLinkedCADFormats,
            FacetingFactor = FacetingFactor,

            Files = ListBoxItems
                .Select(cont => cont.Content.ToString() ?? string.Empty)
                .ToList()
        };

        private ObservableCollection<string> _configs = [];
        public ObservableCollection<string> Configs
        {
            get => _configs;
            set => SetProperty(ref _configs, value);
        }

        private RelayCommand _loadConfigsCommand;
        public RelayCommand LoadConfigsCommand => _loadConfigsCommand ??= new RelayCommand(_ => LoadConfig());
        private void LoadConfig()
        {
            using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            IEnumerable<string> configs = File.ReadLines(openFileDialog.FileName);
            Configs = new ObservableCollection<string>(
                configs.Where(e => !Configs.Any(c => c == e) && e.EndsWith(".json")));

            if (!Configs.Any())
                MessageBox.Show("В текстовом файле не было найдено подходящей информации");
        }

        private RelayCommand _raiseBatchEventCommand;
        public RelayCommand RaiseBatchEventCommand => _raiseBatchEventCommand ??=
            new RelayCommand(obj => _eventHandlerNWCExportBatchUiArg.Raise(this));

        NavisworksParameters IConfigNWC.Parameters => _selectedParameters.Key;
        NavisworksCoordinates IConfigNWC.Coordinates => _selectedCoordinates.Key;
    }
}