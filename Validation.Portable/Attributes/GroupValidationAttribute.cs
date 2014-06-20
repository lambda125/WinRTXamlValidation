// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupValidationAttribute.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;

    using WinRTXAMLValidation.Library.Core;
    using WinRTXAMLValidation.Library.Core.Extensions;

    /// <summary>
    /// Base class for group validation as as inheritance of the <see cref="ExtendedValidationAttribute"/> class.
    /// </summary>
    public abstract class GroupValidationAttribute : ExtendedValidationAttribute, IGroupValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupValidationAttribute"/> class.
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
        protected GroupValidationAttribute(
            ValidationLevel validationLevel = ValidationLevel.Error,
            bool useInImplicitValidation = true,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true)
            : base(validationLevel, useInImplicitValidation, showMessageOnProperty, showMessageInSummary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupValidationAttribute"/> class by using the function that enables access to validation resources.
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
        protected GroupValidationAttribute(
            Func<string> errorMessageAccessor,
            ValidationLevel validationLevel = ValidationLevel.Error,
            bool useInImplicitValidation = true,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true)
            : base(
                errorMessageAccessor,
                validationLevel,
                useInImplicitValidation,
                showMessageOnProperty,
                showMessageInSummary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupValidationAttribute"/> class by using the error message to associate with a validation control.
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
        protected GroupValidationAttribute(
            string errorMessage,
            ValidationLevel validationLevel = ValidationLevel.Error,
            bool useInImplicitValidation = true,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true)
            : base(errorMessage, validationLevel, useInImplicitValidation, showMessageOnProperty, showMessageInSummary)
        {
        }

        /// <summary>
        /// Gets the property names in the group of the validation attribute that are affected by the rule.
        /// For those properties, a validation message will be shown.
        /// </summary>
        public IEnumerable<string> AffectedPropertyNames
        {
            get
            {
                return this.AffectedProperties.Select(exp => exp.ExtractPropertyName());
            }
        }

        /// <summary>
        /// Gets the property names in the group of the validation attribute which affect this rule validity.
        /// The value changing of any of those properties will cause the validation to run.
        /// </summary>
        public IEnumerable<string> CausativePropertyNames
        {
            get
            {
                return this.CausativeProperties.Select(exp => exp.ExtractPropertyName());
            }
        }

        /// <summary>
        /// Gets the property expressions in the group of the validation attribute that are affected by the rule.
        /// For those properties, a validation message will be shown.
        /// </summary>
        protected abstract IEnumerable<Expression<Func<object>>> AffectedProperties { get; }

        /// <summary>
        /// Gets the property expressions in the group of the validation attribute which affect this rule validity.
        /// The value changing of any of those properties will cause the validation to run.
        /// </summary>
        protected abstract IEnumerable<Expression<Func<object>>> CausativeProperties { get; }

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
    }
}
