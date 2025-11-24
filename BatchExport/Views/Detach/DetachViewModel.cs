using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Resources;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Detach;

public class DetachViewModel : ViewModelBase, IConfigDetach
{
    private bool _checkForEmptyView = true;
    private bool _removeLinks = true;
    private bool _isToRename;
    private bool _removeEmptyWorksets;
    private bool _purge;

    private int _radioButtonMode;

    private string _maskIn = Strings.MaskIn;
    private string _maskOut = Strings.MaskOut;
    private string _maskInName = "R18";
    private string _maskOutName = "R24";

    private RelayCommand _radioButtonCommand;

    public DetachViewModel(EventHandlerDetach eventHandlerDetach)
    {
        EventHandlerBase = eventHandlerDetach;
        HelpMessage = string.Join(Environment.NewLine,
            Strings.HelpDetachTitle,
            Strings.HelpLoad,
            Strings.HelpFolder,
            Strings.HelpDetachMid,
            Strings.HelpList,
            Strings.HelpStart);

#if R22_OR_GREATER
        IsWorksetRemoverEnabled = true;
        _removeEmptyWorksets = true;
#endif
        IsPurgeEnabled = true;
        _purge = true;
    }

    [UsedImplicitly]
    public int RadioButtonMode
    {
        get => _radioButtonMode;
        set => SetProperty(ref _radioButtonMode, value);
    }

    public override RelayCommand RadioButtonCommand => _radioButtonCommand ??= new RelayCommand(GetRbCommand);

    [UsedImplicitly]
    public string MaskIn
    {
        get => _maskIn;
        set => SetProperty(ref _maskIn, value);
    }

    [UsedImplicitly]
    public string MaskOut
    {
        get => _maskOut;
        set => SetProperty(ref _maskOut, value);
    }

    [UsedImplicitly] public bool IsPurgeEnabled { get; }

    [UsedImplicitly] public bool IsWorksetRemoverEnabled { get; }

    [UsedImplicitly]
    public bool Purge
    {
        get => _purge;
        set => SetProperty(ref _purge, value);
    }

    public bool CheckForEmptyView
    {
        get => _checkForEmptyView;
        set => SetProperty(ref _checkForEmptyView, value);
    }

    public bool IsToRename
    {
        get => _isToRename;
        set => SetProperty(ref _isToRename, value);
    }

    public string MaskInName
    {
        get => _maskInName;
        set => SetProperty(ref _maskInName, value);
    }

    public string MaskOutName
    {
        get => _maskOutName;
        set => SetProperty(ref _maskOutName, value);
    }

    public bool RemoveLinks
    {
        get => _removeLinks;
        set => SetProperty(ref _removeLinks, value);
    }

    public bool RemoveEmptyWorksets
    {
        get => _removeEmptyWorksets;
        set => SetProperty(ref _removeEmptyWorksets, value);
    }

    private void GetRbCommand(object parameter)
    {
        _radioButtonMode = (string)parameter switch
        {
            "Folder" => 1,
            "Mask" => 2,
            _ => _radioButtonMode
        };
    }
}