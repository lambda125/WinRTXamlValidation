// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncValidationAttribute.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Threading.Tasks;

    using WinRTXAMLValidation.Library.Core;
    using WinRTXAMLValidation.Library.Core.Helpers;

    /// <summary>
    /// Validation attribute class to enable async validation.
    /// </summary>
    public abstract class AsyncValidationAttribute : Attribute, IValidationAttribute
    {
        /// <summary>
        /// The error message accessor function.
        /// </summary>
        private Func<string> errorMessageAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncValidationAttribute"/> class.
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
        protected AsyncValidationAttribute(
            ValidationLevel validationLevel = ValidationLevel.Error,
            bool useInImplicitValidation = true,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true)
        {
            this.ValidationLevel = validationLevel;
            this.UseInImplicitValidation = useInImplicitValidation;
            this.ShowMessageOnProperty = showMessageOnProperty;
            this.ShowMessageInSummary = showMessageInSummary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncValidationAttribute"/> class by using the function that enables access to validation resources.
        /// </summary>
        /// <param name="errorMessageAccessor">
        /// Function that enables access to validation message resources.
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
        protected AsyncValidationAttribute(
            Func<string> errorMessageAccessor,
            ValidationLevel validationLevel = ValidationLevel.Error,
            bool useInImplicitValidation = true,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true)
            : this(validationLevel, useInImplicitValidation, showMessageOnProperty, showMessageInSummary)
        {
            Guard.AssertNotNull(errorMessageAccessor, "errorMessageAccessor");

            this.errorMessageAccessor = errorMessageAccessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncValidationAttribute"/> class by using the error message to associate with a validation control.
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
        protected AsyncValidationAttribute(
            string errorMessage,
            ValidationLevel validationLevel = ValidationLevel.Error,
            bool useInImplicitValidation = true,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true)
            : this(validationLevel, useInImplicitValidation, showMessageOnProperty, showMessageInSummary)
        {
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets the validation level (Error, Warning).
        /// </summary>
        public ValidationLevel ValidationLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this attribute should be used when validation
        /// is performed in a property's setter (implicit validation). Otherwise, validation can
        /// be triggered explicitly by calling the ValidateAsync() method on the corresponding
        /// <see cref="ValidationBindableBase"/> object.
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
        /// Gets or sets an error message to associate with a validation control if validation fails.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                lock (this)
                {
                    return this.errorMessageAccessor != null
                        ? this.ErrorMessageString ?? (this.ErrorMessageString = this.errorMessageAccessor())
                        : this.ErrorMessageString;
                }
            }

            set
            {
                lock (this)
                {
                    this.errorMessageAccessor = null;
                    this.ErrorMessageString = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the error message resource name to use in order to look up the <see cref="ErrorMessageResourceType"/> property value if validation fails.
        /// </summary>
        public string ErrorMessageResourceName { get; set; }

        /// <summary>
        /// Gets or sets the resource type to use for error-message lookup if validation fails.
        /// </summary>
        public Type ErrorMessageResourceType { get; set; }

        /// <summary>
        /// Gets the localized validation error message.
        /// </summary>
        protected string ErrorMessageString { get; private set; }

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name"> The name to include in the formatted message. </param>
        /// <returns> An instance of the formatted error message. </returns>
        public virtual string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentUICulture, this.ErrorMessage, name);
        }

        /// <summary>
        /// Checks whether the specified value is valid with respect to the current validation attribute.
        /// </summary>
        /// <param name="value"> The value to validate. </param>
        /// <param name="validationContext"> The context information about the validation operation. </param>
        /// <returns> The <see cref="Task"/> determining an instance of the <see cref="ValidationResult"/> class. </returns>
        public async Task<ValidationResult> GetValidationResultAsync(object value, ValidationContext validationContext)
        {
            Guard.AssertNotNull(validationContext, "validationContext");

            return await this.IsValidAsync(value, validationContext);
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="value"> The object to validate. </param>
        /// <param name="validationContext">The <see cref="ValidationContext"/> object that describes the context where the validation checks are performed. This parameter cannot be null.</param>
        /// <returns> The <see cref="Task"/> performing the validation. </returns>
        public async Task ValidateAsync(object value, ValidationContext validationContext)
        {
            Guard.AssertNotNull(validationContext, "validationContext");

            var validationResult = await this.IsValidAsync(value, validationContext);
            if (validationResult != ValidationResult.Success)
            {
                throw new AsyncValidationException(validationResult, this, value);
            }
        }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value"> The value to validate. </param>
        /// <param name="validationContext"> The context information about the validation operation. </param>
        /// <returns> The <see cref="Task"/> determining an instance of the <see cref="ValidationResult"/> class. </returns>
        protected abstract Task<ValidationResult> IsValidAsync(object value, ValidationContext validationContext);
    }
}
