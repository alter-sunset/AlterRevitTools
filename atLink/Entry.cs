using AlterTools.Utils.Interfaces;
using AlterTools.Utils.MVVM;
using Autodesk.Revit.DB;

namespace AlterTools.atLink;

public class Entry : NotifyPropertyChanged, ISelectable
{
    private readonly ViewModelMain _viewModelMain;

    private bool _isSelected;
    private ImportPlacement _selectedImportPlacement;
    private Workset _selectedWorkset;

    public Entry(ViewModelMain viewModelMain, string name)
    {
        Name = name;
        _viewModelMain = viewModelMain;

        ImportPlacements = ViewModelMain.ImportPlacements;
        SelectedImportPlacement = ImportPlacement.Shared;
        Worksets = _viewModelMain.Worksets;
        SelectedWorkset = Worksets.FirstOrDefault();
    }

    public string Name { get; }

    public ImportPlacement[] ImportPlacements { get; set; }

    public ImportPlacement SelectedImportPlacement
    {
        get => _selectedImportPlacement;
        set
        {
            if (value == _selectedImportPlacement) return;

            SetProperty(ref _selectedImportPlacement, value);

            if (IsSelected)
            {
                _viewModelMain.UpdateSelectedEntries(this, false);
            }
        }
    }

    public Workset[] Worksets { get; set; }

    public Workset SelectedWorkset
    {
        get => _selectedWorkset;
        set
        {
            if (value == _selectedWorkset) return;

            SetProperty(ref _selectedWorkset, value);

            if (IsSelected)
            {
                _viewModelMain.UpdateSelectedEntries(this, true);
            }
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}