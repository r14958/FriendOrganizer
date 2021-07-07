using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ValueConverters
{
    /// <summary>
    /// Converts an incoming DateTimeOffset value to a DateTime and vice versa.  If an error occurs,
    /// will simply pass the original value.
    /// </summary>
    class DateTimeOffsetToDateTimeConverter : ValueConverterBase<DateTimeOffsetToDateTimeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTimeOffset input)
            {
                return input.DateTime;
            }
            else
            {
                return value;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is DateTime input)
            {
                return new DateTimeOffset(input);
            }
            else
            {
                return value;
            }
            
        }
    }
}
