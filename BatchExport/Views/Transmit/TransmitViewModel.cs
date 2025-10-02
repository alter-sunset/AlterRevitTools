using System;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Transmit;

public class TransmitViewModel : ViewModelBase
{
    private bool _isSameFolder;

    public TransmitViewModel(EventHandlerTransmit eventHandlerTransmit)
    {
        EventHandlerBase = eventHandlerTransmit;
        HelpMessage = string.Join(Environment.NewLine,
            Resources.Strings.Help_Load,
            Resources.Strings.Help_Folder,
            Resources.Strings.Help_List,
            Resources.Strings.Help_Start);
    }

    public bool IsSameFolder
    {
        get => _isSameFolder;
        set => SetProperty(ref _isSameFolder, value);
    }
}