using AlterTools.atExportModel.Interfaces;
using AlterTools.Utils.MVVM;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Windows;

public class ViewModelNWC : NotifyPropertyChanged
{
    public IConfigNWC Config { get; set; }

    public ViewModelNWC(IConfigNWC config)
    {
        Config = config;

        SelectedCoordinates = Coordinates.First(x => x.Key == Config.Coordinates);
        SelectedParameters = Parameters.First(x => x.Key == Config.Parameters);
    }

    public bool ConvertElementProperties
    {
        get => Config.ConvertElementProperties;
        set
        {
            if (Config.ConvertElementProperties == value) return;
            Config.ConvertElementProperties = value;
            OnPropertyChanged();
        }
    }

    public bool DivideFileIntoLevels
    {
        get => Config.DivideFileIntoLevels;
        set
        {
            if (Config.DivideFileIntoLevels == value) return;

            Config.DivideFileIntoLevels = value;
            OnPropertyChanged();
        }
    }

    public bool ExportElementIds
    {
        get => Config.ExportElementIds;
        set
        {
            if (Config.ExportElementIds == value) return;

            Config.ExportElementIds = value;
            OnPropertyChanged();
        }
    }

    public bool ExportLinks
    {
        get => Config.ExportLinks;
        set
        {
            if (Config.ExportLinks == value) return;

            Config.ExportLinks = value;
            OnPropertyChanged();
        }
    }

    public bool ExportParts
    {
        get => Config.ExportParts;
        set
        {
            if (Config.ExportParts == value) return;

            Config.ExportParts = value;
            OnPropertyChanged();
        }
    }

    public bool ExportRoomAsAttribute
    {
        get => Config.ExportRoomAsAttribute;
        set
        {
            if (Config.ExportRoomAsAttribute == value) return;

            Config.ExportRoomAsAttribute = value;
            OnPropertyChanged();
        }
    }

    public bool ExportRoomGeometry
    {
        get => Config.ExportRoomGeometry;
        set
        {
            if (Config.ExportRoomGeometry == value) return;

            Config.ExportRoomGeometry = value;
            OnPropertyChanged();
        }
    }

    public bool ExportUrls
    {
        get => Config.ExportUrls;
        set
        {
            if (Config.ExportUrls == value) return;

            Config.ExportUrls = value;
            OnPropertyChanged();
        }
    }

    public bool FindMissingMaterials
    {
        get => Config.FindMissingMaterials;
        set
        {
            if (Config.FindMissingMaterials == value) return;

            Config.FindMissingMaterials = value;
            OnPropertyChanged();
        }
    }

    public bool ConvertLinkedCADFormats
    {
        get => Config.ConvertLinkedCADFormats;
        set
        {
            if (Config.ConvertLinkedCADFormats == value) return;

            Config.ConvertLinkedCADFormats = value;
            OnPropertyChanged();
        }
    }

    public bool ConvertLights
    {
        get => Config.ConvertLights;
        set
        {
            if (Config.ConvertLights == value) return;

            Config.ConvertLights = value;
            OnPropertyChanged();
        }
    }

    public double FacetingFactor
    {
        get => Config.FacetingFactor;
        set
        {
            if (Math.Abs(Config.FacetingFactor - value) < 0.01) return;

            Config.FacetingFactor = value;
            OnPropertyChanged();
        }
    }

    public static IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates => NWCContext.Coordinates;

    public KeyValuePair<NavisworksCoordinates, string> SelectedCoordinates
    {
        get => Coordinates.First(x => x.Key == Config.Coordinates);
        set
        {
            if (Config.Coordinates == value.Key) return;

            Config.Coordinates = value.Key;
            OnPropertyChanged();
        }
    }

    public static IReadOnlyDictionary<NavisworksParameters, string> Parameters => NWCContext.Parameters;

    public KeyValuePair<NavisworksParameters, string> SelectedParameters
    {
        get => Parameters.First(x => x.Key == Config.Parameters);
        set
        {
            if (Config.Parameters == value.Key) return;

            Config.Parameters = value.Key;
            OnPropertyChanged();
        }
    }
}