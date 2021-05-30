using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ValueConverters
{
    class DateTimeOffsetToDateTimeConverter : ValueConverterBase<DateTimeOffsetToDateTimeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTimeOffset)value).DateTime; 
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DateTimeOffset((DateTime)value);
        }
    }
}
