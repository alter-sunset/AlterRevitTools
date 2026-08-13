using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Enums;
using AlterTools.Utils.MVVM;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.UI;

public class ViewModelIFC : NotifyPropertyChanged
{
    public ConfigIFC Config { get; set; }
    public ConfigIFCAdd ConfigAdd { get; set; }

    public ViewModelIFC(ConfigIFC config, ConfigIFCAdd configAdd)
    {
        Config = config;
        ConfigAdd = configAdd;

        SelectedIFCVersion = IFCVersions
            .First(x => x.Key == config.FileVersion);
        SelectedSpaceBoundaryLevel = SpaceBoundaryLevels
            .First(x => x.Key == config.SpaceBoundaryLevel);
        SelectedIFCFileType = IFCFileTypes
            .First(x => x.Key == configAdd.IFCFileType);
        SelectedSitePlacement = SitePlacements
            .First(x => x.Key == configAdd.SitePlacement);
        SelectedLevelOfDetail = LevelsOfDetail
            .First(x => x.Key == configAdd.TessellationLevelOfDetail);
    }

    public static IReadOnlyDictionary<IFCVersion, string> IFCVersions => ContextIFC.IFCVersions;

    public KeyValuePair<IFCVersion, string> SelectedIFCVersion
    {
        get => IFCVersions.First(x => x.Key == Config.FileVersion);
        set
        {
            if (Config.FileVersion == value.Key) return;

            Config.FileVersion = value.Key;
            OnPropertyChanged();
        }
    }

    public static IReadOnlyDictionary<SpaceBoundaryLevel, string> SpaceBoundaryLevels => ContextIFC.SpaceBoundaryLevels;

    public KeyValuePair<SpaceBoundaryLevel, string> SelectedSpaceBoundaryLevel
    {
        get => SpaceBoundaryLevels.First(x => x.Key == Config.SpaceBoundaryLevel);
        set
        {
            if (Config.SpaceBoundaryLevel == value.Key) return;

            Config.SpaceBoundaryLevel = value.Key;
            OnPropertyChanged();
        }
    }

    public static IReadOnlyDictionary<IFCFileType, string> IFCFileTypes => ContextIFC.IFCFileTypes;

    public KeyValuePair<IFCFileType, string> SelectedIFCFileType
    {
        get => IFCFileTypes.First(x => x.Key == ConfigAdd.IFCFileType);
        set
        {
            if (ConfigAdd.IFCFileType == value.Key) return;
            ConfigAdd.IFCFileType = value.Key;

            OnPropertyChanged();
        }
    }

    public static IReadOnlyDictionary<SitePlacement, string> SitePlacements => ContextIFC.SitePlacements;

    public KeyValuePair<SitePlacement, string> SelectedSitePlacement
    {
        get => SitePlacements.First(x => x.Key == ConfigAdd.SitePlacement);
        set
        {
            if (ConfigAdd.SitePlacement == value.Key) return;
            ConfigAdd.SitePlacement = value.Key;

            OnPropertyChanged();
        }
    }

    public static IReadOnlyDictionary<double, string> LevelsOfDetail => ContextIFC.LevelsOfDetail;

    public KeyValuePair<double, string> SelectedLevelOfDetail
    {
        get => LevelsOfDetail.First(x => x.Key == ConfigAdd.TessellationLevelOfDetail);
        set
        {
            if (ConfigAdd.TessellationLevelOfDetail == value.Key) return;
            ConfigAdd.TessellationLevelOfDetail = value.Key;

            OnPropertyChanged();
        }
    }

    private RelayCommand _browseUserDefinedPsetsCommand;
    private RelayCommand _browseUserMappingCommand;

    public string ExportUserDefinedPsetsFileName
    {
        get => ConfigAdd.ExportUserDefinedPsetsFileName;
        set
        {
            if (ConfigAdd.ExportUserDefinedPsetsFileName == value) return;
            ConfigAdd.ExportUserDefinedPsetsFileName = value;

            OnPropertyChanged();
        }
    }

    public string FamilyMappingFile
    {
        get => Config.FamilyMappingFile;
        set
        {
            if (Config.FamilyMappingFile == value) return;
            Config.FamilyMappingFile = value;

            OnPropertyChanged();
        }
    }

    public RelayCommand BrowseUserDefinedPsetsCommand =>
        _browseUserDefinedPsetsCommand ??= new RelayCommand(_ => BrowseFilePset());

    public RelayCommand BrowseUserMappingCommand =>
        _browseUserMappingCommand ??= new RelayCommand(_ => BrowseFileMapping());

    private void BrowseFilePset() =>
        ExportUserDefinedPsetsFileName = VmHelper.BrowseFile(ExportUserDefinedPsetsFileName);

    private void BrowseFileMapping() => FamilyMappingFile = VmHelper.BrowseFile(FamilyMappingFile);
}