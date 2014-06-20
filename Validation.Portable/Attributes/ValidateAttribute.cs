using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WinRTXAMLValidation.Library.Core;

namespace WinRTXAMLValidation.Library.Attributes
{
    /// <summary>
    /// Validation attribute to validate the a property of type T where T : ValidationBindableBase
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidateAttribute : AsyncValidationAttribute
    {
        protected override async Task<ValidationResult> IsValidAsync(object value, ValidationContext validationContext)
        {
            var validatable = value as ValidationBindableBase;

            if (validatable == null)
            {
                return ValidationResult.Success;
            }

            var isValid = await validatable.ValidateAsync();

            return isValid
                ? ValidationResult.Success
                : new ValidationResult(validationContext.MemberName + " has one or more validation errors");
        }
    }
}
