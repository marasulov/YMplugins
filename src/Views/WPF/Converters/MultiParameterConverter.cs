using System.Globalization;
using System.Windows.Data;

namespace YMplugins.Views.Converters;

public class MultiParameterConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        System.Diagnostics.Debug.WriteLine($"Value 1: {values[0]}, Value 2: {values[1]}");
        return values.Clone();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}