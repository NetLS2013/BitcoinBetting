namespace BitcoinBetting.Server.Services.Betting
{
    using System;
    using System.Threading.Tasks;

    using BitcoinBetting.Server.Models.Betting;
    using BitcoinBetting.Server.Services.Contracts;

    using Quartz;

    public class CreateBettingJob : IJob
    {
        private IBettingService bettingService;

        private TimeSpan time = TimeSpan.FromDays(15);

        public CreateBettingJob(IBettingService bettingService)
        {
            this.bettingService = bettingService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var betting = new BettingModel();
            betting.StartDate = DateTime.Now.Date;
            betting.FinishDate = DateTime.Now.Add(this.time);

            decimal? exchangeRate;

            do
            {
                exchangeRate = this.bettingService.CurrentExchange;
            }
            while (!exchangeRate.HasValue);

            betting.ExchangeRate = exchangeRate.Value;

            this.bettingService.Create(betting);

            return Task.CompletedTask;
        }
    }
}