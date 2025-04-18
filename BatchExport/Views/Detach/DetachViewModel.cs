﻿using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Detach
{
    public class DetachViewModel : ViewModelBase, IConfigDetach
    {
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

        private int _radioButtonMode;
        public int RadioButtonMode
        {
            get => _radioButtonMode;
            set => SetProperty(ref _radioButtonMode, value);
        }
        private RelayCommand _radioButtonCommand;
        public override RelayCommand RadioButtonCommand => _radioButtonCommand ??= new RelayCommand(RB_Command);
        private void RB_Command(object parameter)
        {
            _radioButtonMode = (string)parameter switch
            {
                "Folder" => 1,
                "Mask" => 2,
                _ => _radioButtonMode
            };
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

        private bool _isToRename;
        public bool IsToRename
        {
            get => _isToRename;
            set => SetProperty(ref _isToRename, value);
        }

        private string _maskInName = "R18";
        public string MaskInName
        {
            get => _maskInName;
            set => SetProperty(ref _maskInName, value);
        }

        private string _maskOutName = "R24";
        public string MaskOutName
        {
            get => _maskOutName;
            set => SetProperty(ref _maskOutName, value);
        }

        private bool _removeLinks = true;
        public bool RemoveLinks
        {
            get => _removeLinks;
            set => SetProperty(ref _removeLinks, value);
        }

        private bool _removeEmptyWorksets;
        public bool RemoveEmptyWorksets
        {
            get => _removeEmptyWorksets;
            set => SetProperty(ref _removeEmptyWorksets, value);
        }

        private bool _purge;
        public bool Purge
        {
            get => _purge;
            set => SetProperty(ref _purge, value);
        }

        public bool IsPurgeEnabled { get; }

        public bool IsWorksetRemoverEnabled { get; }
    }
}