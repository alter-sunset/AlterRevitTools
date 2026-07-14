using AlterTools.BatchExport.Views.Base;
using AlterTools.Utils.Interfaces;

namespace AlterTools.BatchExport.Views.NWC;

public class Config(string name) : NotifyPropertyChanged, ISelectable
{
    private bool _isSelected;
    public string Name { get; } = name;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}