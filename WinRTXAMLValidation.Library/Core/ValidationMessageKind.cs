// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationMessageKind.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Core
{
    /// <summary>
    /// The validation message kind enumeration, which contains the kinds of messages a validation can produce.
    /// </summary>
    public enum ValidationMessageKind
    {
        /// <summary>
        /// Specifies a message for a property.
        /// </summary>
        Property,

        /// <summary>
        /// Specifies a message for a whole entity.
        /// </summary>
        Overall
    }
}