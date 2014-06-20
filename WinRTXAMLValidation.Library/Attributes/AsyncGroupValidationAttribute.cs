// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncGroupValidationAttribute.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Library.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using WinRTXAMLValidation.Library.Core;
    using WinRTXAMLValidation.Library.Core.Extensions;

    /// <summary>
    /// Validation attribute class, which combines the async validation behavior of <see cref="AsyncValidationAttribute"/> 
    /// with the group validation of <see cref="IGroupValidationAttribute"/>.
    /// </summary>
    public abstract class AsyncGroupValidationAttribute : AsyncValidationAttribute, IGroupValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncGroupValidationAttribute"/> class.
        /// </summary>
        /// <param name="validationLevel">The validation level (Error, Warning).</param>
        protected AsyncGroupValidationAttribute(ValidationLevel validationLevel = ValidationLevel.Error)
            : base(validationLevel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncGroupValidationAttribute"/> class by using the function that enables access to validation resources.
        /// </summary>
        /// <param name="errorMessageAccessor">
        /// Function that enables access to validation resources.
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
        protected AsyncGroupValidationAttribute(
            Func<string> errorMessageAccessor,
            ValidationLevel validationLevel = ValidationLevel.Error,
            bool useInImplicitValidation = true,
            bool showMessageOnProperty = true,
            bool showMessageInSummary = true)
            : base(errorMessageAccessor, validationLevel, useInImplicitValidation, showMessageOnProperty, showMessageInSummary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncGroupValidationAttribute"/> class by using the error message to associate with a validation control.
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
        protected AsyncGroupValidationAttribute(
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
                return this.AffectedMemberExpressions.Select(exp => exp.ExtractPropertyName());
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
                return this.CausativeMemberExpressions.Select(exp => exp.ExtractPropertyName());
            }
        }

        /// <summary>
        /// Gets the property expressions in the group of the validation attribute that are affected by the rule.
        /// For those properties, a validation message will be shown.
        /// </summary>
        protected abstract IEnumerable<Expression<Func<object>>> AffectedMemberExpressions { get; }

        /// <summary>
        /// Gets the property expressions in the group of the validation attribute which affect this rule validity.
        /// The value changing of any of those properties will cause the validation to run.
        /// </summary>
        protected abstract IEnumerable<Expression<Func<object>>> CausativeMemberExpressions { get; }
    }
}