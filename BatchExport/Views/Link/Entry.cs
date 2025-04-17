using Autodesk.Revit.DB;
using System.Linq;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Link
{
    public class Entry : NotifyPropertyChanged, ISelectable
    {
        private readonly LinkViewModel _viewModel;
        public Entry(LinkViewModel viewModel, string name)
        {
            _name = name;
            _viewModel = viewModel;

            ImportPlacements = LinkViewModel.ImportPlacements;
            SelectedImportPlacement = ImportPlacement.Shared;
            Worksets = _viewModel.Worksets;
            SelectedWorkset = Worksets.FirstOrDefault();
        }

        private readonly string _name;
        public string Name => _name;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public ImportPlacement[] ImportPlacements { get; set; }
        private ImportPlacement _selectedImportPlacement;
        public ImportPlacement SelectedImportPlacement
        {
            get => _selectedImportPlacement;
            set
            {
                if (value == _selectedImportPlacement) return;

                SetProperty(ref _selectedImportPlacement, value);

                if (IsSelected)
                {
                    _viewModel.UpdateSelectedEntries(this, false);
                }
            }
        }

        public Workset[] Worksets { get; set; }
        private Workset _selectedWorkset;
        public Workset SelectedWorkset
        {
            get => _selectedWorkset;
            set
            {
                if (value == _selectedWorkset) return;

                SetProperty(ref _selectedWorkset, value);

                if (IsSelected)
                {
                    _viewModel.UpdateSelectedEntries(this, true);
                }
            }
        }
    }
}