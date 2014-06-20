// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NegatedBooleanConverter.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Demo.Common.Converters
{
    using System;

    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Value converter which negates a boolean value.
    /// </summary>
    public sealed class NegatedBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Inverts a boolean value.
        /// </summary>
        /// <param name="value">the value to be translated</param>
        /// <param name="targetType">the target type to translate</param>
        /// <param name="parameter">parameters for translation</param>
        /// <param name="language">the language to use</param>
        /// <returns>the translation result</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool && (bool)value);
        }

        /// <summary>
        /// Inverts a boolean value.
        /// </summary>
        /// <param name="value">the value to be translated</param>
        /// <param name="targetType">the target type to translate</param>
        /// <param name="parameter">parameters for translation</param>
        /// <param name="language">the language to use</param>
        /// <returns>the translation result</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool && (bool)value);
        }
    }
}
