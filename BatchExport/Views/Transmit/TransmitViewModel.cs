using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.Resources;
using AlterTools.Utils.MVVM;

namespace AlterTools.BatchExport.Views.Transmit;

public class TransmitViewModel : ViewModelBase
{
    private bool _isSameFolder;

    public TransmitViewModel(EventHandlerTransmit eventHandlerTransmit)
    {
        EventHandlerBase = eventHandlerTransmit;
        HelpMessage = string.Join(Environment.NewLine,
            Strings.HelpTransmitTitle,
            Strings.HelpLoad,
            Strings.HelpFolder,
            Strings.HelpList,
            Strings.HelpStart);
    }

    [UsedImplicitly]
    public bool IsSameFolder
    {
        get => _isSameFolder;
        set => SetProperty(ref _isSameFolder, value);
    }
}