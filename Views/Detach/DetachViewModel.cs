using System.Windows.Forms;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Detach
{
    public class DetachViewModel : ViewModelBase
    {
        public DetachViewModel(EventHandlerDetachModelsVMArg eventHandlerDetachModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerDetachModelsUiArg;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessageType.DetachTitle,
                    HelpMessageType.Load,
                    HelpMessageType.Folder,
                    HelpMessageType.DetachMid,
                    HelpMessageType.List,
                    HelpMessageType.Start);
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

        private bool _checkForEmptyView = true;
        public bool CheckForEmptyView
        {
            get => _checkForEmptyView;
            set
            {
                _checkForEmptyView = value;
                OnPropertyChanged(nameof(CheckForEmptyView));
            }
        }

        private bool _isToRename = false;
        public bool IsToRename
        {
            get => _isToRename;
            set
            {
                _isToRename = value;
                OnPropertyChanged(nameof(IsToRename));
            }
        }

        private string _maskInName = "R21";
        public string MaskInName
        {
            get => _maskInName;
            set
            {
                _maskInName = value.Trim();
                OnPropertyChanged(nameof(MaskInName));
            }
        }

        private string _maskOutName = "R25";
        public string MaskOutName
        {
            get => _maskOutName;
            set
            {
                _maskOutName = value.Trim();
                OnPropertyChanged(nameof(MaskOutName));
            }
        }

        private bool _purge = true;
        public bool Purge
        {
            get => _purge;
            set
            {
                _purge = value;
                OnPropertyChanged(nameof(Purge));
            }
        }

        private bool _removeEmptyWorksets = false;
        public bool RemoveEmptyWorksets
        {
            get => _removeEmptyWorksets;
            set
            {
                _removeEmptyWorksets = value;
                OnPropertyChanged(nameof(RemoveEmptyWorksets));
            }
        }
    }
}