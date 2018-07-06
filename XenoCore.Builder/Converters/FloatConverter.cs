using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XenoCore.Builder.Converters
{
    [ValueConversion(typeof(double), typeof(Single))]
    public class FloatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;

            if (value.GetType() == typeof(Single))
                return System.Convert.ToDouble((Single)value);

            return (double)value;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;

            return System.Convert.ToSingle((double)value);

        }
    }
}
