using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Detach
{
    public class DetachViewModel : ViewModelBase
    {
        public DetachViewModel(EventHandlerDetachModelsVMArg eventHandlerDetachModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerDetachModelsUiArg;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessages.DetachTitle,
                    HelpMessages.Load,
                    HelpMessages.Folder,
                    HelpMessages.DetachMid,
                    HelpMessages.List,
                    HelpMessages.Start);
        }

        private int _radioButtonMode = 0;
        public int RadioButtonMode
        {
            get => _radioButtonMode;
            set
            {
                _radioButtonMode = value;
                OnPropertyChanged(nameof(RadioButtonMode));
            }
        }

        private RelayCommand _radioButtonCommand;
        public override RelayCommand RadioButtonCommand
        {
            get
            {
                return _radioButtonCommand ??= new RelayCommand(RB_Command);
            }
        }
        private void RB_Command(object parameter)
        {
            switch ((string)parameter)
            {
                case "Folder":
                    _radioButtonMode = 1;
                    break;
                case "Mask":
                    _radioButtonMode = 2;
                    break;
            }
        }

        private string _maskIn = @"05_В_Работе\52_ПД";
        public string MaskIn
        {
            get => _maskIn;
            set
            {
                _maskIn = value.Trim();
                OnPropertyChanged(nameof(MaskIn));
            }
        }

        private string _maskOut = @"06_Общие\62_ПД";
        public string MaskOut
        {
            get => _maskOut;
            set
            {
                _maskOut = value.Trim();
                OnPropertyChanged(nameof(MaskOut));
            }
        }
    }
}