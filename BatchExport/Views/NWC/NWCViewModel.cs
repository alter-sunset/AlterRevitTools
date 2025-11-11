using System.Collections.ObjectModel;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Resources;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Utils.Extensions;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.NWC;

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
        HelpMessage = string.Join(Environment.NewLine,
            Strings.HelpNWCTitle,
            Strings.HelpLoad,
            Strings.HelpFolder,
            Strings.HelpNaming,
            Strings.HelpConfig,
            Strings.HelpStart,
            Strings.HelpNWCEnd);
    }

    [UsedImplicitly]
    public static IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates => NWCContext.Coordinates;

    [UsedImplicitly]
    public KeyValuePair<NavisworksCoordinates, string> SelectedCoordinates
    {
        get => _selectedCoordinates;
        set => SetProperty(ref _selectedCoordinates, value);
    }

    [UsedImplicitly]
    public static IReadOnlyDictionary<NavisworksParameters, string> Parameters => NWCContext.Parameters;

    [UsedImplicitly]
    public KeyValuePair<NavisworksParameters, string> SelectedParameters
    {
        get => _selectedParameters;
        set => SetProperty(ref _selectedParameters, value);
    }

    [UsedImplicitly]
    public ObservableCollection<Config> Configs
    {
        get => _configs;
        set => SetProperty(ref _configs, value);
    }

    [UsedImplicitly]
    public RelayCommand LoadConfigsCommand => _loadConfigsCommand ??= new RelayCommand(_ => LoadConfig());

    [UsedImplicitly]
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

    private protected override void LoadList()
    {
        OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        using FileStream file = File.OpenRead(openFileDialog.FileName);

        NWCFormDeserializer(JsonHelper<NWCForm>.DeserializeConfig(file));
    }

    public void NWCFormDeserializer(NWCForm form)
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
        WorksetPrefix = string.Join(";", form.WorksetPrefixes);
        ExportScopeView = NavisworksExportScope.View == form.ExportScope;
        ListBoxItems = 
        [
            .. form.Files
                .FilterRevitFiles()
                .Select(DefaultListBoxItem)
        ];
        ConvertLights = form.ConvertLights;
        ConvertLinkedCADFormats = form.ConvertLinkedCADFormats;
        FacetingFactor = form.FacetingFactor;
        SelectedCoordinates = Coordinates.FirstOrDefault(coord => coord.Key == form.Coordinates);
        SelectedParameters = Parameters.FirstOrDefault(param => param.Key == form.Parameters);
        TurnOffLog = form.TurnOffLog;
    }

    private protected override void SaveList()
    {
        using NWCForm form = NWCFormSerializer();
        using SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

        if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

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
            WorksetPrefixes = 
                [
                    .. WorksetPrefix.Split(';')
                        .Select(prefix => prefix.Trim())
                ],
            ConvertLights = ConvertLights,
            ConvertLinkedCADFormats = ConvertLinkedCADFormats,
            FacetingFactor = FacetingFactor,
            Files = [.. ListBoxItems.Select(item => item.Content.ToString())],
            TurnOffLog = TurnOffLog
        };
    }

    private void LoadConfig()
    {
        using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        IEnumerable<string> configs = File.ReadLines(openFileDialog.FileName);

        Configs = 
        [
            .. configs.Where(config => config.EndsWith(".json")
                                       && File.Exists(config))
                .Select(config => new Config(config))
        ];

        if (!Configs.Any())
        {
            MessageBox.Show(NoFiles);
        }
    }

    private protected override void DeleteSelectedItems()
    {
        ListBoxItems.Where(item => item.IsSelected)
            .ToList()
            .ForEach(item => ListBoxItems.Remove(item));

        Configs.Where(config => config.IsSelected)
            .ToList()
            .ForEach(config => Configs.Remove(config));
    }
}