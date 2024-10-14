using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Detach
{
    public class DetachViewModel : ViewModelBase, IConfigDetach
    {
        public DetachViewModel(EventHandlerDetach eventHandlerDetach)
        {
            EventHandlerBase = eventHandlerDetach;
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
            set => SetProperty(ref _radioButtonMode, value);
        }
        private RelayCommand _radioButtonCommand;
        public override RelayCommand RadioButtonCommand => _radioButtonCommand ??= new RelayCommand(RB_Command);
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
            set => SetProperty(ref _maskIn, value);
        }

        private string _maskOut = @"06_Общие\62_ПД";
        public string MaskOut
        {
            get => _maskOut;
            set => SetProperty(ref _maskOut, value);
        }

        private bool _checkForEmptyView = true;
        public bool CheckForEmptyView
        {
            get => _checkForEmptyView;
            set => SetProperty(ref _checkForEmptyView, value);
        }

        private bool _isToRename = false;
        public bool IsToRename
        {
            get => _isToRename;
            set => SetProperty(ref _isToRename, value);
        }

        private string _maskInName = "R21";
        public string MaskInName
        {
            get => _maskInName;
            set => SetProperty(ref _maskInName, value);
        }

        private string _maskOutName = "R25";
        public string MaskOutName
        {
            get => _maskOutName;
            set => SetProperty(ref _maskOutName, value);
        }

        private bool _purge = true;
        public bool Purge
        {
            get => _purge;
            set => SetProperty(ref _purge, value);
        }

        private bool _removeEmptyWorksets = false;
        public bool RemoveEmptyWorksets
        {
            get => _removeEmptyWorksets;
            set => SetProperty(ref _removeEmptyWorksets, value);
        }
    }
}