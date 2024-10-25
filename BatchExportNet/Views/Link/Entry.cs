using Autodesk.Revit.DB;
using BatchExportNet.Views.Base;
using System.ComponentModel;
using VLS.BatchExportNet.Views.Link;

namespace BatchExportNet.Views.Link
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
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public ImportPlacement SelectedOptionalValue
        {
            get => _selectedOptionalValue;
            set
            {
                if (_selectedOptionalValue != value)
                {
                    _selectedOptionalValue = value;
                    OnPropertyChanged(nameof(SelectedOptionalValue));

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
            Name = name;
        }

        public void SetIsUpdating(bool isUpdating)
        {
            IsUpdating = isUpdating;
        }
    }
}
