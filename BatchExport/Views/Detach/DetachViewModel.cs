using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;
using JetBrains.Annotations;

namespace AlterTools.BatchExport.Views.Detach;

public class DetachViewModel : ViewModelBase, IConfigDetach
{
    private bool _checkForEmptyView = true;

    private bool _isToRename;

    private string _maskIn = @"05_В_Работе\52_ПД";

    private string _maskInName = "R18";

    private string _maskOut = @"06_Общие\62_ПД";

    private string _maskOutName = "R24";

    private bool _purge;

    private RelayCommand _radioButtonCommand;

    private int _radioButtonMode;

    private bool _removeEmptyWorksets;

    private bool _removeLinks = true;

    public DetachViewModel(EventHandlerDetach eventHandlerDetach)
    {
        EventHandlerBase = eventHandlerDetach;
        HelpMessage = Help.GetHelpDictionary()
            .GetResultMessage(HelpMessageType.DetachTitle,
                HelpMessageType.Load,
                HelpMessageType.Folder,
                HelpMessageType.DetachMid,
                HelpMessageType.List,
                HelpMessageType.Start);

#if R22_OR_GREATER
        IsWorksetRemoverEnabled = true;
        _removeEmptyWorksets = true;
#endif

#if R24_OR_GREATER
        IsPurgeEnabled = true;
        _purge = true;
#endif
    }

    public int RadioButtonMode
    {
        get => _radioButtonMode;
        set => SetProperty(ref _radioButtonMode, value);
    }

    public override RelayCommand RadioButtonCommand => _radioButtonCommand ??= new RelayCommand(RB_Command);

    public string MaskIn
    {
        get => _maskIn;
        set => SetProperty(ref _maskIn, value);
    }

    public string MaskOut
    {
        get => _maskOut;
        set => SetProperty(ref _maskOut, value);
    }

    [UsedImplicitly]
    public bool IsPurgeEnabled { get; }

    public bool IsWorksetRemoverEnabled { get; }

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

    private void RB_Command(object parameter)
    {
        _radioButtonMode = (string)parameter switch
        {
            "Folder" => 1,
            "Mask" => 2,
            _ => _radioButtonMode
        };
    }
}