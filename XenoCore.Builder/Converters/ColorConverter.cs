using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace XenoCore.Builder.Converters
{
    [ValueConversion(typeof(System.Windows.Media.Color), typeof(Microsoft.Xna.Framework.Color))]
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new System.Windows.Media.Color();

            var c = (Microsoft.Xna.Framework.Color)value;

            return new System.Windows.Media.Color()
            {
                A = c.A,
                B = c.B,
                R = c.R,
                G = c.G
            };


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Microsoft.Xna.Framework.Color.TransparentBlack;

            var c = (System.Windows.Media.Color)value;

            return new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, c.A);
        }
    }
}
