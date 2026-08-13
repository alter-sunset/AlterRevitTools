using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AlterTools.atExportModel.Utils;

public sealed class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Equals(value, parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value
            ? parameter
            : DependencyProperty.UnsetValue;
    }
}