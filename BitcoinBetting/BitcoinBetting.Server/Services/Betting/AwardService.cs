namespace BitcoinBetting.Server.Services.Betting
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using BitcoinBetting.Server.Database.Helpers;
    using BitcoinBetting.Server.Models.Betting;
    using BitcoinBetting.Server.Models.Bitcoin;
    using BitcoinBetting.Server.Services.Bitcoin;
    using BitcoinBetting.Server.Services.Contracts;
    using BitcoinBetting.Server.Services.Identity;

    using Microsoft.AspNetCore.Identity;

    using NBitcoin;

    using Quartz;

    public class AwardService : IJob
    {
        private IBettingService bettingService;

        private IBidService bidService;

        private IWalletService walletService;

        private IBitcoinAverageApi bitcoinAverage;

        private BitcoinWalletService bitcoinWalletService;

        private readonly UserManager<AppIdentityUser> userManager;

        private readonly IEmailSender emailSender;

        public AwardService(IBettingService bettingService, IBidService bidService, IBitcoinAverageApi bitcoinAverage, BitcoinWalletService bitcoinWalletService)
        {
            this.bettingService = bettingService;
            this.bidService = bidService;
            this.bitcoinAverage = bitcoinAverage;
            this.bitcoinWalletService = bitcoinWalletService;
        }

        public async Task PayForWin(int betId)
        {
            var bet = this.bettingService.GetById(betId);

            var sideWin = await this.GetCurrentExchange() > bet.Bank;

            var bankMore = this.GetBankBySide(true, betId);
            var bankLess = this.GetBankBySide(false, betId);

            foreach (var bid in this.bidService.Get(model => model.Side == sideWin && model.BettingId == bet.BettingId))
            {
                var award = sideWin
                                ? this.GetAwardAmount(bid.Amount, bid.Coefficient, bankMore, bankLess)
                                : this.GetAwardAmount(bid.Amount, bid.Coefficient, bankLess, bankMore);

                try
                {
                    this.bitcoinWalletService.Send(bid.PaymentAddress, (decimal)award);

                    var user = await this.userManager.FindByIdAsync(bid.UserId);
                    await this.emailSender.SendEmailAsync(
                        user.Email,
                        "Bitcoin app payment",
                        "You win " + (decimal)award + " on your wallet " + bid.PaymentAddress);
                }
                catch
                {

                }

            }
        }

        public void CheckPayment()
        {
            var bettings = this.bettingService.Get(model => model.Status == BettingStatus.Waiting).Select(model => model.BettingId);
            var bids = this.bidService.Get(
                model => model.PaymentStatus != PaymentStatus.Confirmed && bettings.Contains(model.BettingId));

            foreach (var bid in bids)
            {
                uint256 transaction;
                bid.PaymentStatus = this.bitcoinWalletService.IsPaymentDone(
                    out transaction,
                    BitcoinAddress.Create(this.walletService.GetById(bid.WalletId).Address),
                    BitcoinAddress.Create(bid.PaymentAddress),
                    bid.Amount,
                    bid.Date);

                if (bid.PaymentStatus != PaymentStatus.None)
                {
                    var transactionId = transaction.ToString();
                    var sameBid = this.bidService.Get(model => model.WalletId == bid.WalletId && model.Amount == bid.Amount && model.TransactionId == transactionId);
                    if (sameBid.Any())
                    {
                        bid.TransactionId = transactionId;

                        this.bidService.Update(bid);
                    }
                }
            }
        }

        public double GetAwardAmount(int bidId)
        {
            var bid = this.bidService.GetById(bidId);

            return BettingHelper.GetAmountPayment(
                bid.Amount,
                bid.Coefficient,
                this.GetBankBySide(bid.Side, bid.BettingId),
                this.GetBankBySide(!bid.Side, bid.BettingId));
        }

        public double GetAwardAmount(double amount, double coefficient, double currBank, double oppositeBank)
        {
            return BettingHelper.GetAmountPayment(
                amount,
                coefficient,
                currBank,
                oppositeBank);
        }

        public double GetBankBySide(bool side, int betId)
        {
            return this.bidService.Get(model => model.BettingId == betId && model.Side == side)?.Sum(model => model.Amount) ?? 0;
        }

        public async Task<double?> GetCurrentExchange()
        {
            return (await this.bitcoinAverage.GetShortDataAsync())?["BTCUSD"]?["averages"]?["day"]?.ToObject<double>();
        }

        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}