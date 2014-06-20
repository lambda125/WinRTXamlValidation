// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedValidationAttribute.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using WinRTXAMLValidation.Library.Core;
    using WinRTXAMLValidation.Library.Core.Helpers;

    /// <summary>
    /// A base validation attribute class which extends the <see cref="ValidationAttribute"/> class 
    /// by adding additional information like validation level, implicit validation, and more.
    /// </summary>
    public abstract class ExtendedValidationAttribute : ValidationAttribute, IValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedValidationAttribute"/> class.
        /// </summary>
        /// <param name="validationLevel">
        /// The validation level (Error, Warning).
        /// </param>
        /// <param name="useInImplicitValidation">
        /// Indicates if the validation of this attribute will be triggered by a property change.
        /// </param>
        /// <param name="showMessageOnProperty">
        /// Indicates whether the validation message will be shown on the property at the UI.
        /// </param>
        /// <param name="showMessageInSummary">
        /// Indicates whether the validation message will be shown in the validation summary at the UI.
        /// </param>
        protected ExtendedValidationAttribute(
            ValidationLevel validationLevel = ValidationLevel.Error,
            bool useInImplicitValidation = true,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true)
        {
            this.SetBasicProperties(validationLevel, useInImplicitValidation, showMessageOnProperty, showMessageInSummary);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedValidationAttribute"/> class by using the function that enables access to validation resources.
        /// </summary>
        /// <param name="errorMessageAccessor">
        /// Function that enables access to validation message resources.</param>
        /// <param name="validationLevel">
        /// The validation level (Error, Warning).
        /// </param>
        /// <param name="useInImplicitValidation">
        /// Indicates if the validation of this attribute will be triggered by a property change.
        /// </param>
        /// <param name="showMessageOnProperty">
        /// Indicates whether the validation message will be shown on the property at the UI.
        /// </param>
        /// <param name="showMessageInSummary">
        /// Indicates whether the validation message will be shown in the validation summary at the UI.
        /// </param>
        protected ExtendedValidationAttribute(
            Func<string> errorMessageAccessor,
            ValidationLevel validationLevel = ValidationLevel.Error,
            bool useInImplicitValidation = true,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true)
            : base(errorMessageAccessor)
        {
            Guard.AssertNotNull(errorMessageAccessor, "errorMessageAccessor");

            this.SetBasicProperties(validationLevel, useInImplicitValidation, showMessageOnProperty, showMessageInSummary);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedValidationAttribute"/> class by using the error message to associate with a validation control.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message to associate with a validation control.
        /// </param>
        /// <param name="validationLevel">
        /// The validation level (Error, Warning).
        /// </param>
        /// <param name="useInImplicitValidation">
        /// Indicates if the validation of this attribute will be triggered by a property change.
        /// </param>
        /// <param name="showMessageOnProperty">
        /// Indicates whether the validation message will be shown on the property at the UI.
        /// </param>
        /// <param name="showMessageInSummary">
        /// Indicates whether the validation message will be shown in the validation summary at the UI.
        /// </param>
        protected ExtendedValidationAttribute(
            string errorMessage,
            ValidationLevel validationLevel = ValidationLevel.Error,
            bool useInImplicitValidation = true,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true)
            : base(errorMessage)
        {
            this.SetBasicProperties(validationLevel, useInImplicitValidation, showMessageOnProperty, showMessageInSummary);
        }

        /// <summary>
        /// Gets or sets the validation level.
        /// </summary>
        public ValidationLevel ValidationLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this attribute should be used when validation
        /// is performed in a property's setter.
        /// </summary>
        public bool UseInImplicitValidation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the validation message for this attribute
        /// should be shown on the property field at the UI (corresponding ValidationPanel).
        /// </summary>
        public bool ShowMessageOnProperty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the validation message for this attribute
        /// should be shown on the validation summary at the UI (corresponding ValidationSummary).
        /// </summary>
        public bool ShowMessageInSummary { get; set; }

        /// <summary>
        /// Validates the specified value in the given validation context.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns> 
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> 
        /// class as result of the validation process.
        /// </returns>
        protected abstract override ValidationResult IsValid(object value, ValidationContext validationContext);

        /// <summary>
        /// Sets all basic properties of the attribute.
        /// </summary>
        /// <param name="validationLevel">The validation level.</param>
        /// <param name="useInImplicitValidation">The value indicating if the attribute should be evaluated in implicit validation.</param>
        /// <param name="showMessageOnProperty">Indicates if the message should be shown on the property.</param>
        /// <param name="showMessageInSummary">Indicates if the message should be shown in the validation summary.</param>
        private void SetBasicProperties(
            ValidationLevel validationLevel,
            bool useInImplicitValidation,
            bool showMessageOnProperty,
            bool showMessageInSummary)
        {
            this.ValidationLevel = validationLevel;
            this.UseInImplicitValidation = useInImplicitValidation;
            this.ShowMessageOnProperty = showMessageOnProperty;
            this.ShowMessageInSummary = showMessageInSummary;
        }
    }
}
