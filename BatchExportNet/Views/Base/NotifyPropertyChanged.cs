using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VLS.BatchExportNet.Views.Base
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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