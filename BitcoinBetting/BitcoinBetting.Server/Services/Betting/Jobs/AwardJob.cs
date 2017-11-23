namespace BitcoinBetting.Server.Services.Betting
{
    using System.Linq;
    using System.Threading.Tasks;

    using BitcoinBetting.Server.Database.Helpers;
    using BitcoinBetting.Server.Models.Betting;
    using BitcoinBetting.Server.Services.Bitcoin;
    using BitcoinBetting.Server.Services.Contracts;
    using BitcoinBetting.Server.Services.Identity;

    using Microsoft.AspNetCore.Identity;

    using Quartz;

    public class AwardService : IJob
    {
        private IBettingService bettingService;

        private IBidService bidService;

        private IWalletService walletService;

        private BitcoinWalletService bitcoinWalletService;

        private readonly UserManager<AppIdentityUser> userManager;

        private readonly IEmailSender emailSender;

        public AwardService(
            IBettingService bettingService,
            IBidService bidService,
            BitcoinWalletService bitcoinWalletService)
        {
            this.bettingService = bettingService;
            this.bidService = bidService;
            this.bitcoinWalletService = bitcoinWalletService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var bet = this.bettingService.Get(model => model.Status == BettingStatus.Waiting)
                .OrderBy(model => model.FinishDate).FirstOrDefault();

            if (bet != null)
            {
                bet.Status = BettingStatus.Done;

                decimal? currentExchange;

                do
                {
                    currentExchange = this.bettingService.CurrentExchange;
                }
                while (!currentExchange.HasValue);

                var sideWin = currentExchange.Value > bet.Bank;

                var bankMore = this.bettingService.GetBank(bet.BettingId, true);
                var bankLess = this.bettingService.GetBank(bet.BettingId, false);

                foreach (var bid in this.bidService.Get(
                    model => model.Side == sideWin && model.BettingId == bet.BettingId && !model.Status))
                {
                    var award = sideWin
                                    ? BettingHelper.GetAmountPayment(bid.Amount, bid.Coefficient, bankMore, bankLess)
                                    : BettingHelper.GetAmountPayment(bid.Amount, bid.Coefficient, bankLess, bankMore);

                    try
                    {
                        var address = this.walletService.GetById(bid.BidId);
                        if (address != null)
                        {
                            this.bitcoinWalletService.Send(address.Address, award);

                            var user = await this.userManager.FindByIdAsync(bid.UserId);
                            await this.emailSender.SendEmailAsync(
                                user.Email,
                                "Bitcoin app payment",
                                "You win " + (decimal)award + " on your wallet " + bid.PaymentAddress);

                            bid.Status = true;

                            this.bidService.Update(bid);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
    }
}