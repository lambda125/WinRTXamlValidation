// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuctionBid.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Demo.Model
{
    using System.ComponentModel.DataAnnotations;

    using WinRTXAMLValidation.Demo.Model.ValidationAttributes;
    using WinRTXAMLValidation.Library.Core;

    /// <summary>
    /// The AuctionBid model entity contains properties for a current bid 
    /// and a new bid by a user. The class and properties are decorated with several
    /// validation attributes to define a correct auction bid.
    /// </summary>
    [ValidateAgainstMaxBid(ShowMessageOnProperty = false)]
    public class AuctionBid : ValidationBindableBase
    {
        #region Private Fields

        /// <summary>
        /// The current bid value returned by a server.
        /// </summary>
        private double currentBid;

        /// <summary>
        /// The new user bid.
        /// </summary>
        private double newBid;

        /// <summary>
        /// The highest user bid.
        /// </summary>
        private double? maxNewBid;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current auction bid returned by a server.
        /// </summary>
        public double CurrentBid
        {
            get { return this.currentBid; }
            set { this.SetProperty(ref this.currentBid, value); }
        }

        /// <summary>
        /// Gets or sets the new user bid value.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Value must be greater 0.")]
        [ValidateGreaterCurrentBid]
        [ValidateNoBidAbuse]
        [ValidatePossibleMisentryBid(ValidationLevel = ValidationLevel.Warning)]
        [ValidateActualCurrentBid(UseInImplicitValidation = false)]
        public double NewBid
        {
            get { return this.newBid; }
            set { this.SetProperty(ref this.newBid, value); }
        }

        /// <summary>
        /// Gets or sets the highest user bid value.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Value must be greater 0.")]
        [ValidateGreaterCurrentBid]
        public double? MaxNewBid
        {
            get { return this.maxNewBid; }
            set { this.SetProperty(ref this.maxNewBid, value); }
        }

        #endregion
    }
}
