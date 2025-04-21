using System.Linq;
using AlterTools.BatchExport.Views.Base;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.Link
{
    public class Entry : NotifyPropertyChanged, ISelectable
    {
        private readonly LinkViewModel _viewModel;

        private bool _isSelected;
        private ImportPlacement _selectedImportPlacement;
        private Workset _selectedWorkset;

        public Entry(LinkViewModel viewModel, string name)
        {
            Name = name;
            _viewModel = viewModel;

            ImportPlacements = LinkViewModel.ImportPlacements;
            SelectedImportPlacement = ImportPlacement.Shared;
            Worksets = _viewModel.Worksets;
            SelectedWorkset = Worksets.FirstOrDefault();
        }

        public string Name { get; }

        public ImportPlacement[] ImportPlacements { get; set; }

        public ImportPlacement SelectedImportPlacement
        {
            get => _selectedImportPlacement;
            set
            {
                if (value == _selectedImportPlacement)
                {
                    return;
                }

                SetProperty(ref _selectedImportPlacement, value);

                if (IsSelected)
                {
                    _viewModel.UpdateSelectedEntries(this, false);
                }
            }
        }

        public Workset[] Worksets { get; set; }

        public Workset SelectedWorkset
        {
            get => _selectedWorkset;
            set
            {
                if (value == _selectedWorkset)
                {
                    return;
                }

                SetProperty(ref _selectedWorkset, value);

                if (IsSelected)
                {
                    _viewModel.UpdateSelectedEntries(this, true);
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}