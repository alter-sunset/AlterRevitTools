﻿using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.NWC
{
    public class Config(string name) : NotifyPropertyChanged, ISelectable
    {
        private readonly string _name = name;
        public string Name => _name;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
