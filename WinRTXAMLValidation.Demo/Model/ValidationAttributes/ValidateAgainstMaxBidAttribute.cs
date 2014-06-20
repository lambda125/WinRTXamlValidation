// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateAgainstMaxBidAttribute.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Demo.Model.ValidationAttributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq.Expressions;

    using WinRTXAMLValidation.Demo.Model;
    using WinRTXAMLValidation.Library.Attributes;

    /// <summary>
    /// Validation attribute to validate the new bid value against the max bid value.
    /// This is a group validation attribute to decorate the whole entity instead of single properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateAgainstMaxBidAttribute : GroupValidationAttribute
    {
        protected override IEnumerable<Expression<Func<object>>> AffectedProperties
        {
            get
            {
                AuctionBid entity = null;  // Placeholder to get typed entity for Expression
                return new Expression<Func<object>>[]
                    {
                        () => entity.NewBid,
                        () => entity.MaxNewBid
                    };
            }
        }

        protected override IEnumerable<Expression<Func<object>>> CausativeProperties
        {
            get { return this.AffectedProperties; }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // get validation data
            var auctionBid = (AuctionBid)validationContext.ObjectInstance;

            // perform validation
            return auctionBid.MaxNewBid.HasValue && auctionBid.NewBid > auctionBid.MaxNewBid.Value
                ? new ValidationResult("New bid must not be greater than highest bid.")
                : ValidationResult.Success;
        }
    }
}
