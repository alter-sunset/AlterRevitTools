using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AlterTools.BatchExport.Views.Base;

public class NotifyPropertyChanged : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (value is string stringValue)
        {
            value = (T)(object)stringValue.Trim();
        }

        if (EqualityComparer<T>.Default.Equals(field, value)) return;

        field = value;
        OnPropertyChanged(propertyName);
    }
}