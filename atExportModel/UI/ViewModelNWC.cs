using AlterTools.atExportModel.Configs;
using AlterTools.Utils.MVVM;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.UI;

public class ViewModelNWC : NotifyPropertyChanged
{
    public ConfigNWC Config { get; set; }

    public ViewModelNWC(ConfigNWC config)
    {
        Config = config;

        SelectedCoordinates = Coordinates.First(x => x.Key == Config.Coordinates);
        SelectedParameters = Parameters.First(x => x.Key == Config.Parameters);
        SelectedExportScope = ExportScopes.First(x => x.Key == Config.ExportScope);
    }

    public static IReadOnlyDictionary<NavisworksCoordinates, string> Coordinates => ContextNWC.Coordinates;

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

    public static IReadOnlyDictionary<NavisworksParameters, string> Parameters => ContextNWC.Parameters;

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

    public static IReadOnlyDictionary<NavisworksExportScope, string> ExportScopes => ContextNWC.ExportScopes;

    public KeyValuePair<NavisworksExportScope, string> SelectedExportScope
    {
        get => ExportScopes.First(x => x.Key == Config.ExportScope);
        set
        {
            if (Config.ExportScope == value.Key) return;

            Config.ExportScope = value.Key;
            OnPropertyChanged();
        }
    }
}