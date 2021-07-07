using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FriendOrganizer.UI.ValueConverters
{
    /// <summary>
    /// Substitutes a human-readable phrase if the original value bound to a text box control 
    /// is null or an empty string. One-way from source only.
    /// </summary>
    public class TextBoxOriginalValueConverter : IValueConverter
    {
        public const string Blank_Phrase = "<blank>";
        
        /// <summary>
        /// Substitutes a human-readable phrase if the original value bound to a text box control 
        /// is null or an empty string.
        /// </summary>
        /// <param name="value">The incoming value which may be text, an empty string, or null.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>The incoming text or a substitute phrase.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.ToString() == string.Empty)
            {
                return Blank_Phrase;
            }

            return value.ToString();
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
