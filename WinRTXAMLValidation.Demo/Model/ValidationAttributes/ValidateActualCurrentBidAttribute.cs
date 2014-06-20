// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateActualCurrentBidAttribute.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Demo.Model.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    using WinRTXAMLValidation.Demo.Model;
    using WinRTXAMLValidation.Demo.Model.Services;
    using WinRTXAMLValidation.Library.Attributes;

    /// <summary>
    /// Validation attribute to validate the new bid against the actual current bid
    /// that is retrieved from a server.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateActualCurrentBidAttribute : AsyncValidationAttribute
    {
        protected override async Task<ValidationResult> IsValidAsync(object value, ValidationContext validationContext)
        {
            // get validation data
            var newBid = (double)value;
            var auctionBid = (AuctionBid)validationContext.ObjectInstance;
            
            // get actual current bid value
            var service = new AuctionService();
            var currentBid = await service.GetCurrentBidAsync(auctionBid);

            // perform validation
            return (newBid > auctionBid.CurrentBid && newBid <= currentBid)
                ? new ValidationResult("Your bid has been surpassed.")
                : ValidationResult.Success;
        }
    }
}
