// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatePossibleMisentryBidAttribute.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Demo.Model.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using WinRTXAMLValidation.Demo.Model;
    using WinRTXAMLValidation.Library.Attributes;

    /// <summary>
    /// Validation attribute to validate that the new bid is no possible misentry by the user.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePossibleMisentryBidAttribute : ExtendedValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // get validation data
            var newBid = (double)value;
            var auctionBid = (AuctionBid)validationContext.ObjectInstance;
            
            // for this demo, the new bid is classified as possible misentry, if it surpasses the current bid by a 100 times
            return newBid > auctionBid.CurrentBid * 100
                ? new ValidationResult("Your bid surpasses the current bid by a 100 times. Are you sure?")
                : ValidationResult.Success;
        }
    }
}
