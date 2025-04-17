﻿using Autodesk.Revit.DB;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.NWC
{
    public class NWC_ViewModel : ViewModelBaseExtended, IConfigNWC
    {
        private readonly EventHandlerNWC_Batch _eventHandlerNWC_Batch;

        public NWC_ViewModel(EventHandlerNWC_Batch eventHandlerNWC_Batch, EventHandlerNWC eventHandlerNWC)
        {
            _eventHandlerNWC_Batch = eventHandlerNWC_Batch;
            EventHandlerBase = eventHandlerNWC;
            HelpMessage = Help.GetHelpDictionary()
                              .GetResultMessage(HelpMessageType.NwcTitle,
                                                HelpMessageType.Load,
                                                HelpMessageType.Folder,
                                                HelpMessageType.Naming,
                                                HelpMessageType.Config,
                                                HelpMessageType.Start,
                                                HelpMessageType.NwcEnd);
        }

        private bool _convertElementProperties;
        public bool ConvertElementProperties
        {
            get => _convertElementProperties;
            set => SetProperty(ref _convertElementProperties, value);
        }

        private readonly IReadOnlyDictionary<NavisworksCoordinates, string> _coordinates = NWC_Context.Coordinates;
        public IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates => _coordinates;

        private KeyValuePair<NavisworksCoordinates, string> _selectedCoordinates = NWC_Context.Coordinates.FirstOrDefault(coord => coord.Key == NavisworksCoordinates.Shared);
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

        private bool _exportLinks;
        public bool ExportLinks
        {
            get => _exportLinks;
            set => SetProperty(ref _exportLinks, value);
        }

        private bool _exportParts;
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

        private bool _exportRoomGeometry;
        public bool ExportRoomGeometry
        {
            get => _exportRoomGeometry;
            set => SetProperty(ref _exportRoomGeometry, value);
        }

        private bool _exportUrls;
        public bool ExportUrls
        {
            get => _exportUrls;
            set => SetProperty(ref _exportUrls, value);
        }

        private bool _findMissingMaterials;
        public bool FindMissingMaterials
        {
            get => _findMissingMaterials;
            set => SetProperty(ref _findMissingMaterials, value);
        }

        private readonly IReadOnlyDictionary<NavisworksParameters, string> _parameters = NWC_Context.Parameters;
        private KeyValuePair<NavisworksParameters, string> _selectedParameters = NWC_Context.Parameters.FirstOrDefault(param => param.Key is NavisworksParameters.All);
        public IReadOnlyDictionary<NavisworksParameters, string> Parameters => _parameters;
        public KeyValuePair<NavisworksParameters, string> SelectedParameters
        {
            get => _selectedParameters;
            set => SetProperty(ref _selectedParameters, value);
        }

        private bool _convertLinkedCADFormats;
        public bool ConvertLinkedCADFormats
        {
            get => _convertLinkedCADFormats;
            set => SetProperty(ref _convertLinkedCADFormats, value);
        }

        private bool _convertLights;
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

            if (DialogResult.OK != openFileDialog.ShowDialog()) return;

            using FileStream file = File.OpenRead(openFileDialog.FileName);

            NWCFormDeserilaizer(JsonHelper<NWCForm>.DeserializeConfig(file));
        }
        public void NWCFormDeserilaizer(NWCForm form)
        {
            if (null == form) return;

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

            IEnumerable<string> files = form.Files.FilterRevitFiles();

            ListBoxItems = new ObservableCollection<ListBoxItem>(files.Select(DefaultListBoxItem));

            ConvertLights = form.ConvertLights;
            ConvertLinkedCADFormats = form.ConvertLinkedCADFormats;
            FacetingFactor = form.FacetingFactor;
            SelectedCoordinates = _coordinates.FirstOrDefault(coord => coord.Key == form.Coordinates);
            SelectedParameters = _parameters.FirstOrDefault(param => param.Key == form.Parameters);
        }

        private RelayCommand _saveListCommand;
        public override RelayCommand SaveListCommand => _saveListCommand ??= new RelayCommand(_ => SaveList());
        private void SaveList()
        {
            using NWCForm form = NWCFormSerializer();
            using SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

            if (DialogResult.OK != saveFileDialog.ShowDialog()) return;

            string fileName = saveFileDialog.FileName;

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

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

            WorksetPrefixes = WorksetPrefix.Split(';')
                                           .Select(prefix => prefix.Trim())
                                           .ToArray(),

            ConvertLights = ConvertLights,
            ConvertLinkedCADFormats = ConvertLinkedCADFormats,
            FacetingFactor = FacetingFactor,

            Files = ListBoxItems.Select(item => item.Content.ToString() ?? string.Empty)
                                .ToArray()
        };

        private ObservableCollection<Config> _configs = [];
        public ObservableCollection<Config> Configs
        {
            get => _configs;
            set => SetProperty(ref _configs, value);
        }

        private RelayCommand _loadConfigsCommand;
        public RelayCommand LoadConfigsCommand => _loadConfigsCommand ??= new RelayCommand(_ => LoadConfig());
        private void LoadConfig()
        {
            using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();

            if (DialogResult.OK != openFileDialog.ShowDialog()) return;

            IEnumerable<string> configs = File.ReadLines(openFileDialog.FileName);

            Configs = new ObservableCollection<Config>(configs.Where(config => config.EndsWith(".json") && File.Exists(config))
                                                              .Select(config => new Config(config)));

            if (!Configs.Any())
            {
                MessageBox.Show(NO_FILES);
            }
        }

        private RelayCommand _raiseBatchEventCommand;
        public RelayCommand RaiseBatchEventCommand => _raiseBatchEventCommand ??= new RelayCommand(_ => _eventHandlerNWC_Batch.Raise(this));

        NavisworksParameters IConfigNWC.Parameters => _selectedParameters.Key;
        NavisworksCoordinates IConfigNWC.Coordinates => _selectedCoordinates.Key;

        private RelayCommand _deleteCommand;
        public override RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(_ => DeleteSelectedItems());
        private void DeleteSelectedItems()
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