using AlterTools.atExportModel.Configs;
using AlterTools.Utils.MVVM;

namespace AlterTools.atExportModel.UI;

public class ViewModelClean : NotifyPropertyChanged
{
    private bool _updating;
    public ConfigClean Config { get; set; }

    public ViewModelClean(ConfigClean config)
    {
        Config = config;
    }

    public bool? All
    {
        get => Config.ConfigRemoveViews.All;
        set
        {
            if (Config.ConfigRemoveViews.All == value) return;

            Config.ConfigRemoveViews.All = value;
            OnPropertyChanged();

            if (_updating || value is null) return;

            _updating = true;

            ThreeDViews = value.Value;
            AreaPlans = value.Value;
            CeilingPlans = value.Value;
            Details = value.Value;
            DraftingViews = value.Value;
            Elevations = value.Value;
            FloorPlans = value.Value;
            ColumnSchedules = value.Value;
            Legends = value.Value;
            Renderings = value.Value;
            Schedules = value.Value;
            Sections = value.Value;
            EngineeringPlans = value.Value;
            Walkthroughs = value.Value;

            _updating = false;
        }
    }

    public bool ThreeDViews
    {
        get => Config.ConfigRemoveViews.ThreeDViews;
        set
        {
            if (Config.ConfigRemoveViews.ThreeDViews == value) return;

            Config.ConfigRemoveViews.ThreeDViews = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool AreaPlans
    {
        get => Config.ConfigRemoveViews.AreaPlans;
        set
        {
            if (Config.ConfigRemoveViews.AreaPlans == value) return;

            Config.ConfigRemoveViews.AreaPlans = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool CeilingPlans
    {
        get => Config.ConfigRemoveViews.CeilingPlans;
        set
        {
            if (Config.ConfigRemoveViews.CeilingPlans == value) return;

            Config.ConfigRemoveViews.CeilingPlans = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool Details
    {
        get => Config.ConfigRemoveViews.Details;
        set
        {
            if (Config.ConfigRemoveViews.Details == value) return;

            Config.ConfigRemoveViews.Details = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool DraftingViews
    {
        get => Config.ConfigRemoveViews.DraftingViews;
        set
        {
            if (Config.ConfigRemoveViews.DraftingViews == value) return;

            Config.ConfigRemoveViews.DraftingViews = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool Elevations
    {
        get => Config.ConfigRemoveViews.Elevations;
        set
        {
            if (Config.ConfigRemoveViews.Elevations == value) return;

            Config.ConfigRemoveViews.Elevations = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool FloorPlans
    {
        get => Config.ConfigRemoveViews.FloorPlans;
        set
        {
            if (Config.ConfigRemoveViews.FloorPlans == value) return;

            Config.ConfigRemoveViews.FloorPlans = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool ColumnSchedules
    {
        get => Config.ConfigRemoveViews.ColumnSchedules;
        set
        {
            if (Config.ConfigRemoveViews.ColumnSchedules == value) return;

            Config.ConfigRemoveViews.ColumnSchedules = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool Legends
    {
        get => Config.ConfigRemoveViews.Legends;
        set
        {
            if (Config.ConfigRemoveViews.Legends == value) return;

            Config.ConfigRemoveViews.Legends = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool Renderings
    {
        get => Config.ConfigRemoveViews.Renderings;
        set
        {
            if (Config.ConfigRemoveViews.Renderings == value) return;

            Config.ConfigRemoveViews.Renderings = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool Schedules
    {
        get => Config.ConfigRemoveViews.Schedules;
        set
        {
            if (Config.ConfigRemoveViews.Schedules == value)
                return;

            Config.ConfigRemoveViews.Schedules = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool Sections
    {
        get => Config.ConfigRemoveViews.Sections;
        set
        {
            if (Config.ConfigRemoveViews.Sections == value) return;

            Config.ConfigRemoveViews.Sections = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool EngineeringPlans
    {
        get => Config.ConfigRemoveViews.EngineeringPlans;
        set
        {
            if (Config.ConfigRemoveViews.EngineeringPlans == value)
                return;

            Config.ConfigRemoveViews.EngineeringPlans = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    public bool Walkthroughs
    {
        get => Config.ConfigRemoveViews.Walkthroughs;
        set
        {
            if (Config.ConfigRemoveViews.Walkthroughs == value) return;

            Config.ConfigRemoveViews.Walkthroughs = value;
            OnPropertyChanged();
            UpdateAll();
        }
    }

    private void UpdateAll()
    {
        if (_updating) return;

        bool[] values =
        [
            ThreeDViews,
            AreaPlans,
            CeilingPlans,
            Details,
            DraftingViews,
            Elevations,
            FloorPlans,
            ColumnSchedules,
            Legends,
            Renderings,
            Schedules,
            Sections,
            EngineeringPlans,
            Walkthroughs
        ];

        _updating = true;

        All = values.All(v => v)
            ? true
            : values.All(v => !v)
                ? false
                : (bool?)null;

        _updating = false;
    }
}