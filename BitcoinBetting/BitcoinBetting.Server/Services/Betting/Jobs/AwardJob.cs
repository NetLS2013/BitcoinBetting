namespace BitcoinBetting.Server.Services.Betting
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using BitcoinBetting.Enum;
    using BitcoinBetting.Server.Helpers;
    using BitcoinBetting.Server.Models.Betting;
    using BitcoinBetting.Server.Models.Bitcoin;
    using BitcoinBetting.Server.Services.Bitcoin;
    using BitcoinBetting.Server.Services.Contracts;
    using BitcoinBetting.Server.Services.Identity;

    using Microsoft.AspNetCore.Identity;

    using Quartz;

    public class AwardJob : IJob
    {
        private IBettingService bettingService;

        private IBidService bidService;

        private IWalletService walletService;

        private BitcoinWalletService bitcoinWalletService;

        private readonly UserManager<AppIdentityUser> userManager;

        private readonly IEmailSender emailSender;

        public AwardJob(IBettingService bettingService, IBidService bidService, IWalletService walletService, BitcoinWalletService bitcoinWalletService, UserManager<AppIdentityUser> userManager, IEmailSender emailSender)
        {
            this.bettingService = bettingService;
            this.bidService = bidService;
            this.walletService = walletService;
            this.bitcoinWalletService = bitcoinWalletService;
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

        private int BasicAddressId = 0;

        public async Task Execute(IJobExecutionContext context)
        {
            var bet = this.bettingService.Get(model => model.Status == BettingStatus.Waiting)
                .OrderBy(model => model.FinishDate).FirstOrDefault();

            if (bet != null)
            {
                decimal? currentExchange;

                do
                {
                    currentExchange = this.bettingService.CurrentExchange;
                }
                while (!currentExchange.HasValue);

                var sideWin = currentExchange.Value > bet.ExchangeRate;

                // Set status and who win
                bet.Result = sideWin;
                bet.Status = BettingStatus.Done;
                await this.bettingService.Update(bet);

                var bankMore = this.bettingService.GetBank(bet.BettingId, true);
                var bankLess = this.bettingService.GetBank(bet.BettingId, false);

                var bids = this.bidService.Get(
                    model => model.BettingId == bet.BettingId && !model.Status
                             && model.PaymentStatus == PaymentStatus.Confirmed);
               
                // select ids of addresses where can be coins
                var ids = bids.Select(model => model.BidId).ToList();

                // use first address as basic
                ids.Add(this.BasicAddressId);

                foreach (var bid in bids)
                {
                    if (bid.Side == sideWin)
                    {
                        var award = sideWin
                                        ? BettingHelper.GetAmountPayment(bid.Amount, bid.Coefficient, bankMore, bankLess)
                                        : BettingHelper.GetAmountPayment(bid.Amount, bid.Coefficient, bankLess, bankMore);

                        try
                        {
                            var address = this.walletService.GetById(bid.WalletId);
                            if (address != null)
                            {
                                // send to winners and send remainder sum to first address
                                this.bitcoinWalletService.Send(address.Address, award, ids, this.BasicAddressId);

                                bid.Paid = true;
                                

                                var user = await this.userManager.FindByIdAsync(bid.UserId);
                                await this.emailSender.SendEmailAsync(
                                    user.Email,
                                    "Bitcoin app payment",
                                    "You win " + (decimal)award + " on your wallet " + bid.PaymentAddress);
                            }
                        }
                        catch (Exception e)
                        {
                            // ignored
                        }
                    }

                    bid.Status = true;

                    this.bidService.Update(bid);
                }
            }
        }
    }
}