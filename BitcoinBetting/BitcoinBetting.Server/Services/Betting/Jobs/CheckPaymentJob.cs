﻿namespace BitcoinBetting.Server.Services.Betting
{
    using System.Linq;
    using System.Threading.Tasks;

    using BitcoinBetting.Server.Models.Betting;
    using BitcoinBetting.Server.Models.Bitcoin;
    using BitcoinBetting.Server.Services.Bitcoin;
    using BitcoinBetting.Server.Services.Contracts;

    using NBitcoin;

    using Quartz;

    public class CheckPaymentJob : IJob
    {
        private IBettingService bettingService;

        private IBidService bidService;

        private BitcoinWalletService bitcoinWalletService;

        public CheckPaymentJob(IBettingService bettingService, IBidService bidService, BitcoinWalletService bitcoinWalletService)
        {
            this.bettingService = bettingService;
            this.bidService = bidService;
            this.bitcoinWalletService = bitcoinWalletService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var bettings = this.bettingService.Get(model => model.Status == BettingStatus.Waiting)
                ?.Select(model => model.BettingId).ToList();

            if (bettings != null && bettings.Any())
            {
                var bids = this.bidService.Get(
                    model => model.PaymentStatus != PaymentStatus.Confirmed && bettings.Contains(model.BettingId))?.ToList();

                if (bids != null && bids.Any())
                {
                    var bidsIds = bids.Select(model => model.BidId);

                    var balances = this.bitcoinWalletService.GetBalances1(bidsIds, out _, out _).ToList();

                    for (int i = 0; i < bids.Count; i++)
                    {
                        if (balances[i].ConfirmMoney > Money.Zero)
                        {
                            bids[i].Amount = balances[i].ConfirmMoney.ToDecimal(MoneyUnit.BTC);
                            bids[i].PaymentStatus = PaymentStatus.Confirmed;

                        }
                        if (balances[i].UnconfirmMoney > Money.Zero)
                        {
                            bids[i].Amount = balances[i].UnconfirmMoney.ToDecimal(MoneyUnit.BTC);
                            bids[i].PaymentStatus = PaymentStatus.Unconfirmed;
                        }

                        this.bidService.Update(bids[i]);
                    }
                }
            }
            //foreach (var bid in bids)
            //{
            //    if (balances[])
            //    {
            //        balances
            //    }

            //    //uint256 transaction;
            //    //bid.PaymentStatus = this.bitcoinWalletService.IsPaymentDone(
            //    //    out transaction,
            //    //    BitcoinAddress.Create(this.walletService.GetById(bid.WalletId).Address),
            //    //    BitcoinAddress.Create(bid.PaymentAddress),
            //    //    bid.Amount,
            //    //    bid.Date);

            //    //if (bid.PaymentStatus != PaymentStatus.None)
            //    //{
            //    //    var transactionId = transaction.ToString();
            //    //    var sameBid = this.bidService.Get(model => model.WalletId == bid.WalletId && model.Amount == bid.Amount && model.TransactionId == transactionId);
            //    //    if (sameBid.Any())
            //    //    {
            //    //        bid.TransactionId = transactionId;

            //    //        this.bidService.Update(bid);
            //    //    }
            //    //}
            //}

            return Task.CompletedTask;
        }
    }
}