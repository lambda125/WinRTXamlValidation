// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateGreaterCurrentBidAttribute.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Demo.Model.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using WinRTXAMLValidation.Demo.Model;

    /// <summary>
    /// Validation attribute to validate that the new/highest bid are greater than the current bid.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateGreaterCurrentBidAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // get validation data
            var newBid = (double?)value;
            var auctionBid = (AuctionBid)validationContext.ObjectInstance;
            
            // perform validation
            return newBid.HasValue && newBid.Value <= auctionBid.CurrentBid 
                ? new ValidationResult("Value must be greater than current bid (" + auctionBid.CurrentBid + ").")
                : ValidationResult.Success;
        }
    }
}
