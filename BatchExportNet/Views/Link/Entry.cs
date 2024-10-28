using Autodesk.Revit.DB;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Link
{
    public class Entry : IEntry, INotifyPropertyChanged
    {
        private string _name;
        private ImportPlacement _selectedOptionalValue;
        private readonly LinkViewModel _viewModel;
        public bool IsUpdating;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public ImportPlacement SelectedOptionalValue
        {
            get => _selectedOptionalValue;
            set
            {
                if (_selectedOptionalValue != value)
                {
                    SetProperty(ref _selectedOptionalValue, value);
                    if (IsSelected)
                        _viewModel.UpdateSelectedEntries(this);
                }
            }
        }

        public ImportPlacement[] OptionalValues { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Entry(LinkViewModel viewModel, string name)
        {
            _viewModel = viewModel;
            OptionalValues = viewModel.OptionalValues;
            SelectedOptionalValue = ImportPlacement.Shared;
            Name = name;
        }
        public void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (value is string stringValue)
                value = (T)(object)stringValue.Trim();

            if (EqualityComparer<T>.Default.Equals(field, value)) return;

            field = value;
            OnPropertyChanged(propertyName);
        }
    }
}
