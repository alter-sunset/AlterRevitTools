using AlterTools.BatchExport.Utils.Extensions;

namespace AlterTools.BatchExport.Views.Base;

public class ViewModelBaseExtended : ViewModelBase, IConfigBaseExtended
{
    private bool _exportScopeView = true;
    private bool _ignoreMissingView;
    private bool _turnOffLog;

    private string _namePostfix = string.Empty;
    private string _namePrefix = string.Empty;

    private string _worksetPrefix = string.Empty;

    [UsedImplicitly]
    public string WorksetPrefix
    {
        get => _worksetPrefix;
        set => SetProperty(ref _worksetPrefix, value);
    }

    public string NamePrefix
    {
        get => _namePrefix;
        set => SetProperty(ref _namePrefix, value);
    }

    public string NamePostfix
    {
        get => _namePostfix;
        set => SetProperty(ref _namePostfix, value);
    }

    public bool ExportScopeView
    {
        get => _exportScopeView;
        set
        {
            _exportScopeView = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ExportScopeWhole));
        }
    }

    public bool IgnoreMissingView
    {
        get => _ignoreMissingView;
        set => SetProperty(ref _ignoreMissingView, value);
    }

    public bool ExportScopeWhole
    {
        get => !_exportScopeView;
        set
        {
            _exportScopeView = !value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ExportScopeView));
        }
    }

    public bool TurnOffLog
    {
        get => _turnOffLog;
        set => SetProperty(ref _turnOffLog, value);
    }

    public string[] WorksetPrefixes => _worksetPrefix.SplitBySemicolon();
}