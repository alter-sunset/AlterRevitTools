using System.Collections.Generic;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Detach
{
    public class DetachViewModel : ViewModelBase
    {
        public DetachViewModel(EventHandlerDetachModelsVMArg eventHandlerDetachModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerDetachModelsUiArg;
            Dictionary<HelpMessages, string> help = Help.GetHelpMessages();
            string _helpMessage =
                help.GetResultMessage(HelpMessages.DetachTitle,
                    HelpMessages.Load,
                    HelpMessages.Folder,
                    HelpMessages.DetachMid,
                    HelpMessages.List,
                    HelpMessages.Start);
            HelpMessage = _helpMessage;
        }

        private int _radioButtonMode = 0;
        public int RadionButtonMode
        {
            get => _radioButtonMode;
            set
            {
                _radioButtonMode = value;
                OnPropertyChanged("RadioButtonMode");
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
                _maskIn = value;
                OnPropertyChanged("MaskIn");
            }
        }

        private string _maskOut = @"06_Общие\62_ПД";
        public string MaskOut
        {
            get => _maskOut;
            set
            {
                _maskOut = value;
                OnPropertyChanged("MaskOut");
            }
        }
    }
}