// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="-" year="2013">
//   Matthias Jauernig (matthias.jauernig@live.de)
//   Markus Demmler (markus.demmler@sdx-ag.de)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WinRTXAMLValidation.Demo.ViewModel
{
    using System.Windows.Input;

    using WinRTXAMLValidation.Demo.Common;
    using WinRTXAMLValidation.Demo.Model;
    using WinRTXAMLValidation.Demo.Model.Services;

    /// <summary>
    /// This view model is an abstraction of the MainPage and contains properties/commands,
    /// to which the MainPage can bind.
    /// </summary>
    public class MainViewModel : BindableBase
    {
        /// <summary>
        /// Indicates whether a new bid is currently sent by the user.
        /// </summary>
        private bool isSendingBid;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            // create a new bid that validates automatically when a property value is set
            this.Bid = new AuctionBid();
            this.Bid.IsImplicitValidationEnabled = true;

            // fake data
            this.Bid.CurrentBid = 102.95;
        }

        /// <summary>
        /// Gets the instantiated auction bid for data binding.
        /// </summary>
        public AuctionBid Bid { get; private set; }

        /// <summary>
        /// Gets a value indicating whether currently a bid is sent.
        /// </summary>
        public bool IsSendingBid
        {
            get { return this.isSendingBid; }
            private set { this.SetProperty(ref this.isSendingBid, value); }
        }

        /// <summary>
        /// Gets a command which sends the new bid to the server.
        /// </summary>
        public ICommand SendBidCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                    {
                        this.IsSendingBid = true;
                        try
                        {
                            // validate explicitly again and if valid -> send to server
                            bool isValid = await this.Bid.ValidateAsync();
                            if (isValid)
                            {
                                var auctionService = new AuctionService();
                                await auctionService.SendBidAsync(this.Bid);
                            }
                        }
                        finally
                        {
                            this.IsSendingBid = false;
                        }
                    });
            }
        }
    }
}