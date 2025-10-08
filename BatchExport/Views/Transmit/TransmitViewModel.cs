using System;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Resources;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Transmit;

public class TransmitViewModel : ViewModelBase
{
    private bool _isSameFolder;

    public TransmitViewModel(EventHandlerTransmit eventHandlerTransmit)
    {
        EventHandlerBase = eventHandlerTransmit;
        HelpMessage = string.Join(Environment.NewLine,
            Strings.Help_TransmitTitle,
            Strings.Help_Load,
            Strings.Help_Folder,
            Strings.Help_List,
            Strings.Help_Start);
    }

    public bool IsSameFolder
    {
        get => _isSameFolder;
        set => SetProperty(ref _isSameFolder, value);
    }
}