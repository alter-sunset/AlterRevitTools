using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Transmit;

public class TransmitViewModel : ViewModelBase
{
    private bool _isSameFolder;

    public TransmitViewModel(EventHandlerTransmit eventHandlerTransmit)
    {
        EventHandlerBase = eventHandlerTransmit;
        HelpMessage = Help.GetHelpDictionary()
            .GetResultMessage(HelpMessageType.TransmitTitle,
                HelpMessageType.Load,
                HelpMessageType.Folder,
                HelpMessageType.List,
                HelpMessageType.Start);
    }

    public bool IsSameFolder
    {
        get => _isSameFolder;
        set => SetProperty(ref _isSameFolder, value);
    }
}