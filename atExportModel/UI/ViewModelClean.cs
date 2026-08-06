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
        _all = Config.ConfigRemoveViews.All;
        _threeDViews = Config.ConfigRemoveViews.ThreeDViews;
        _areaPlans = Config.ConfigRemoveViews.AreaPlans;
        _ceilingPlans = Config.ConfigRemoveViews.CeilingPlans;
        _details = Config.ConfigRemoveViews.Details;
        _draftingViews = Config.ConfigRemoveViews.DraftingViews;
        _elevations = Config.ConfigRemoveViews.Elevations;
        _floorPlans = Config.ConfigRemoveViews.FloorPlans;
        _columnSchedules = Config.ConfigRemoveViews.ColumnSchedules;
        _legends = Config.ConfigRemoveViews.Legends;
        _renderings = Config.ConfigRemoveViews.Renderings;
        _schedules = Config.ConfigRemoveViews.Schedules;
        _sections = Config.ConfigRemoveViews.Sections;
        _engineeringPlans = Config.ConfigRemoveViews.EngineeringPlans;
        _walkthroughs = Config.ConfigRemoveViews.Walkthroughs;
    }

    private bool? _all;

    public bool? All
    {
        get => _all;
        set
        {
            if (_all == value) return;

            SetProperty(ref _all, value);

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

    private bool _threeDViews;

    public bool ThreeDViews
    {
        get => _threeDViews;
        set
        {
            if (_threeDViews == value)
                return;

            SetProperty(ref _threeDViews, value);

            Config.ConfigRemoveViews.ThreeDViews = value;
            UpdateAll();
        }
    }

    private bool _areaPlans;

    public bool AreaPlans
    {
        get => _areaPlans;
        set
        {
            if (_areaPlans == value)
                return;

            SetProperty(ref _areaPlans, value);
            Config.ConfigRemoveViews.AreaPlans = value;
            UpdateAll();
        }
    }

    private bool _ceilingPlans;

    public bool CeilingPlans
    {
        get => _ceilingPlans;
        set
        {
            if (_ceilingPlans == value)
                return;

            SetProperty(ref _ceilingPlans, value);
            Config.ConfigRemoveViews.CeilingPlans = value;
            UpdateAll();
        }
    }

    private bool _details;

    public bool Details
    {
        get => _details;
        set
        {
            if (_details == value)
                return;

            SetProperty(ref _details, value);
            Config.ConfigRemoveViews.Details = value;
            UpdateAll();
        }
    }

    private bool _draftingViews;

    public bool DraftingViews
    {
        get => _draftingViews;
        set
        {
            if (_draftingViews == value)
                return;

            SetProperty(ref _draftingViews, value);
            Config.ConfigRemoveViews.DraftingViews = value;
            UpdateAll();
        }
    }

    private bool _elevations;

    public bool Elevations
    {
        get => _elevations;
        set
        {
            if (_elevations == value)
                return;

            SetProperty(ref _elevations, value);
            Config.ConfigRemoveViews.Elevations = value;
            UpdateAll();
        }
    }

    private bool _floorPlans;

    public bool FloorPlans
    {
        get => _floorPlans;
        set
        {
            if (_floorPlans == value)
                return;

            SetProperty(ref _floorPlans, value);
            Config.ConfigRemoveViews.FloorPlans = value;
            UpdateAll();
        }
    }

    private bool _columnSchedules;

    public bool ColumnSchedules
    {
        get => _columnSchedules;
        set
        {
            if (_columnSchedules == value)
                return;

            SetProperty(ref _columnSchedules, value);
            Config.ConfigRemoveViews.ColumnSchedules = value;
            UpdateAll();
        }
    }

    private bool _legends;

    public bool Legends
    {
        get => _legends;
        set
        {
            if (_legends == value)
                return;

            SetProperty(ref _legends, value);
            Config.ConfigRemoveViews.Legends = value;
            UpdateAll();
        }
    }

    private bool _renderings;

    public bool Renderings
    {
        get => _renderings;
        set
        {
            if (_renderings == value)
                return;

            SetProperty(ref _renderings, value);
            Config.ConfigRemoveViews.Renderings = value;
            UpdateAll();
        }
    }

    private bool _schedules;

    public bool Schedules
    {
        get => _schedules;
        set
        {
            if (_schedules == value)
                return;

            SetProperty(ref _schedules, value);
            Config.ConfigRemoveViews.Schedules = value;
            UpdateAll();
        }
    }

    private bool _sections;

    public bool Sections
    {
        get => _sections;
        set
        {
            if (_sections == value)
                return;

            SetProperty(ref _sections, value);
            Config.ConfigRemoveViews.Sections = value;
            UpdateAll();
        }
    }

    private bool _engineeringPlans;

    public bool EngineeringPlans
    {
        get => _engineeringPlans;
        set
        {
            if (_engineeringPlans == value)
                return;

            SetProperty(ref _engineeringPlans, value);
            Config.ConfigRemoveViews.EngineeringPlans = value;
            UpdateAll();
        }
    }

    private bool _walkthroughs;

    public bool Walkthroughs
    {
        get => _walkthroughs;
        set
        {
            if (_walkthroughs == value)
                return;

            SetProperty(ref _walkthroughs, value);
            Config.ConfigRemoveViews.Walkthroughs = value;
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