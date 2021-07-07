using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.Windows.Shared;
using System.Windows.Data;
using System.Windows;

namespace FriendOrganizer.UI.ValueConverters
{
    /// <summary>
    /// Converts an incoming <see cref="DateTimeOffset"/> or <see cref="DateTime"/> value into a string in the "FullDateTime"/> 
    /// format of the <see cref="DateTimeEdit"/> control
    /// (dddd, MMMM, d, yyyy h:mm:ss tt: e.g.,"Wednesday, May 16, 2001 3:02:15 AM")
    /// </summary>
    /// <remarks>This allows the original date time value to be captured exactly in the format displayed by the control.</remarks>
    public class DateTimeEditOriginalValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime;

            if (value is DateTimeOffset inputDTO)
            {
                dateTime = inputDTO.DateTime;
            }
            else if (value is DateTime inputDT)
            {
                dateTime = inputDT;
            }
            else
            {
                return value; 
            }

            return dateTime.ToLongDateString() + " " + dateTime.ToLongTimeString();
        }

        /// <summary>
        /// Not utilized.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns><see cref="DependencyProperty.UnsetValue"/>, as recommended by Microsoft (see 
        /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.windows.data.ivalueconverter.convertback"/>).</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
