// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuctionService.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Demo.Model.Services
{
    using System.Threading.Tasks;
    using WinRTXAMLValidation.Library.Core;

    /// <summary>
    /// Fake service class to perform auction bid operations.
    /// </summary>
    public class AuctionService
    {
        /// <summary>
        /// Determines if the new bid value is a bid abuse or a valid bid.
        /// </summary>
        /// <param name="bid">Current bid.</param>
        /// <param name="newBid">New bid value.</param>
        /// <returns>Continuation with the abuse result.</returns>
        public async Task<bool> IsBidAbuseAsync(AuctionBid bid, double newBid)
        {
            if (newBid > 1000 * bid.CurrentBid)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the current bid async.
        /// </summary>
        /// <param name="auctionBid">Bid so far.</param>
        /// <returns>Continuation with the current bid value as result.</returns>
        public async Task<double> GetCurrentBidAsync(AuctionBid auctionBid)
        {
            // Simulates a lengthy, asynchronous computation
            await Task.Delay(1000);
            return auctionBid.CurrentBid + 10;
        }

        /// <summary>
        /// Sends a bid async.
        /// </summary>
        /// <param name="bid">The bid to send.</param>
        /// <returns>Continuation with the send result.</returns>
        public async Task<bool> SendBidAsync(AuctionBid bid)
        {
            // fake direct validation logic
            if (bid.NewBid <= await this.GetCurrentBidAsync(bid))
            {
                bid.ValidationMessages.AddMessage(ValidationLevel.Error, "Your bid has been surpassed.");
                return false;
            }

            if (bid.NewBid >= bid.CurrentBid * 100)
            {
                bid.ValidationMessages.AddMessage(
                    () => bid.NewBid, ValidationLevel.Warning, "Your bid surpasses the current bid by a 100 times.");
            }

            //// here should be the send logic

            return true;
        }
    }
}
