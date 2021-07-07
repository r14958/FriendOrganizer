using FriendOrganizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FriendOrganizer.UI.ValueConverters
{

    /// <summary>
    /// Converts an incoming <see cref="ILookupItem.Id"/> to its <see cref="ILookupItem.DisplayMember"/>.
    /// </summary>
    /// <remarks> Used only with a <see cref="ComboBox"/> whose ItemsSource contains an <see cref="IEnumerable{T}"/> that are of Type <see cref="ILookupItem"/>.
    /// One way from source only.</remarks>
    public class ComboBoxOriginalValueConverter : IValueConverter
    {
        /// <summary>
        /// Converts an incoming <see cref="ILookupItem.Id"/> to its <see cref="ILookupItem.DisplayMember"/>.
        /// </summary>
        /// <param name="value"><see cref="ILookupItem.Id"/></param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter"><see cref="ComboBox"/> object or null</param>
        /// <param name="culture">Not used.</param>
        /// <returns><see cref="ILookupItem.DisplayMember"/></returns>
        /// <remarks>If the <see cref="ComboBox"/> object is not provided or no corresponding <see cref="ILookupItem.DisplayMember"/> 
        /// is found, the original value will be returned.</remarks>
        /// <exception cref="InvalidCastException"></exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Cast the incoming value
            var id = (int)value;

            
            // Make sure the ComboBox has provided its list of LookupItems.
            if (parameter is not ComboBox comboBox || comboBox.ItemsSource == null) return value;

            // Attempt to find the corresponding LookupItem
            var lookupItem = comboBox.ItemsSource.OfType<ILookupItem>().SingleOrDefault(l => l.Id == id);

            // Not found...
            if (lookupItem == null) return value;

            // Item found, return its DisplayMember.
            return lookupItem.DisplayMember;
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
