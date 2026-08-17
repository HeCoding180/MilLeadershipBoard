using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.UI.Converters
{
    /// <summary>
    /// Converts a boolean value to its inverted <see cref="Visibility"/> equivalent.
    /// </summary>
    /// <remarks>
    /// When the bound boolean is <see langword="true"/>, the resulting <see cref="Visibility"/>
    /// is <see cref="Visibility.Collapsed"/>; when <see langword="false"/>, the resulting
    /// value is <see cref="Visibility.Visible"/>. This is the inverse of the
    /// standard BooleanToVisibilityConverter behavior and is typically used to
    /// hide an element while a condition (e.g. IsBusy) is true.
    /// </remarks>
    public class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean source value into an inverted <see cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The source binding value. Expected to be a <see cref="bool"/>.</param>
        /// <param name="targetType">The type of the binding target property. Unused.</param>
        /// <param name="parameter">An optional converter parameter. Unused.</param>
        /// <param name="language">The language of the conversion (BCP-47 tag). Unused.</param>
        /// <returns>
        /// <see cref="Visibility.Collapsed"/> if <paramref name="value"/> is <see langword="true"/>;
        /// <see cref="Visibility.Visible"/> if <paramref name="value"/> is <see langword="false"/> or not a <see cref="bool"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Visible;
        }

        /// <summary>
        /// Converts a <see cref="Visibility"/> value back into its inverted boolean equivalent.
        /// </summary>
        /// <param name="value">The source binding value. Expected to be a <see cref="Visibility"/>.</param>
        /// <param name="targetType">The type of the binding target property. Unused.</param>
        /// <param name="parameter">An optional converter parameter. Unused.</param>
        /// <param name="language">The language of the conversion (BCP-47 tag). Unused.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="value"/> is not <see cref="Visibility.Visible"/>
        /// (i.e. Collapsed or Hidden); <see langword="false"/> if <paramref name="value"/> is
        /// <see cref="Visibility.Visible"/> or not a <see cref="Visibility"/> value.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }

            return false;
        }
    }
}
