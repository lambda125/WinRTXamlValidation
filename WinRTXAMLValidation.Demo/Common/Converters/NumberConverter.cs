// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumberConverter.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Demo.Common.Converters
{
    using System;

    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Value converter to convert a <see cref="Nullable"/> double number for UI binding.
    /// </summary>
    public class NumberConverter : IValueConverter
    {
        /// <summary>
        /// Converts a double? value to string for UI binding.
        /// </summary>
        /// <param name="value">the value to be converted</param>
        /// <param name="targetType">the target type to convert</param>
        /// <param name="parameter">parameters for conversion</param>
        /// <param name="language">the language to use</param>
        /// <returns>the conversion result</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var doubleValue = value as double?;

            if (!doubleValue.HasValue)
            {
                return string.Empty;
            }

            return doubleValue.Value.ToString();
        }

        /// <summary>
        /// Converts a string to a double? value.
        /// </summary>
        /// <param name="value">the value to be converted</param>
        /// <param name="targetType">the target type to convert</param>
        /// <param name="parameter">parameters for conversion</param>
        /// <param name="language">the language to use</param>
        /// <returns>the conversion result</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var stringValue = value as string;

            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }

            try
            {
                return System.Convert.ToDouble(stringValue);
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
