// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGroupValidationAttribute.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Attributes
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for group validation attributes, by which a group of properties of an element can be validated.
    /// </summary>
    public interface IGroupValidationAttribute
    {
        /// <summary>
        /// Gets the property names in the group of the validation attribute that are affected by the rule.
        /// For those properties, a validation message will be shown.
        /// </summary>
        IEnumerable<string> AffectedPropertyNames { get; }

        /// <summary>
        /// Gets the property names in the group of the validation attribute which affect this rule validity.
        /// The value changing of any of those properties will cause the validation to run.
        /// </summary>
        IEnumerable<string> CausativePropertyNames { get; }
    }
}
