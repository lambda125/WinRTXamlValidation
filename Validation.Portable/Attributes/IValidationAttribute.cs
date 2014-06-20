// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidationAttribute.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Attributes
{
    using WinRTXAMLValidation.Library.Core;

    /// <summary>
    /// The base interface for all validation attributes.
    /// </summary>
    public interface IValidationAttribute
    {
        /// <summary>
        /// Gets the validation level (Error, Warning).
        /// </summary>
        ValidationLevel ValidationLevel { get; }

        /// <summary>
        /// Gets a value indicating whether this attribute should be used when validation
        /// is performed in a property's setter (implicit validation). Otherwise, validation can
        /// be triggered explicitly by calling the ValidateAsync() method on the corresponding
        /// <see cref="ValidationBindableBase"/> object.
        /// </summary>
        bool UseInImplicitValidation { get; }

        /// <summary>
        /// Gets a value indicating whether the validation message for this attribute
        /// should be shown on the property field at the UI (corresponding ValidationPanel).
        /// </summary>
        bool ShowMessageOnProperty { get; }

        /// <summary>
        /// Gets a value indicating whether the validation message for this attribute
        /// should be shown on the validation summary at the UI (corresponding ValidationSummary).
        /// </summary>
        bool ShowMessageInSummary { get; }
    }
}
