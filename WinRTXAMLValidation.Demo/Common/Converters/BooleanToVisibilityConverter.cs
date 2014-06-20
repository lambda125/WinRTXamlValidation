// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Demo.Common.Converters
{
    using System;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Translates <c>true</c> to <see cref="Visibility.Visible"/> and <c>false</c> to <see cref="Visibility.Collapsed"/>.
        /// </summary>
        /// <param name="value">the value to be translated</param>
        /// <param name="targetType">the target type to translate</param>
        /// <param name="parameter">parameters for translation</param>
        /// <param name="language">the language to use</param>
        /// <returns>the translation result</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Translates <see cref="Visibility.Visible"/> to <c>true</c> and <see cref="Visibility.Collapsed"/> to <c>false</c>.
        /// </summary>
        /// <param name="value">the value to be translated</param>
        /// <param name="targetType">the target type to translate</param>
        /// <param name="parameter">parameters for translation</param>
        /// <param name="language">the language to use</param>
        /// <returns>the translation result</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
