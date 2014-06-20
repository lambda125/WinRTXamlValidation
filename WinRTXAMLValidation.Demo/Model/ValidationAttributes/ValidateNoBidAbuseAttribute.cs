// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateNoBidAbuseAttribute.cs" company="-" year="2013">
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
    /// Validation attribute to validate that the new bid is no abuse of the auction bid system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateNoBidAbuseAttribute : AsyncValidationAttribute
    {
        protected override async Task<ValidationResult> IsValidAsync(object value, ValidationContext validationContext)
        {
            // get validation data
            var newBid = (double)value;
            var auctionBid = (AuctionBid)validationContext.ObjectInstance;
            
            // determine bid abuse
            var service = new AuctionService();
            var isAbuse = await service.IsBidAbuseAsync(auctionBid, newBid);
            
            return isAbuse
                ? new ValidationResult("Your bid has been detected to be an abuse")
                : ValidationResult.Success;
        }
    }
}
