using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace YMplugins.Views.Converters
{
    public class TupleItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Tuple<long, string> tuple && parameter is string index)
            {
                return index switch
                {
                    "Item1" => tuple.Item1,
                    "Item2" => tuple.Item2,
                    _ => null,
                };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
