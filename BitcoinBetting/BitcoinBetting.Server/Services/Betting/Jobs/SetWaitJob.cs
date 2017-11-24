using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Services.Betting.Jobs
{
    using BitcoinBetting.Server.Models.Betting;
    using BitcoinBetting.Server.Services.Contracts;

    using Quartz;

    public class SetWaitJob : IJob
    {
        private IBettingService bettingService;

        public SetWaitJob(IBettingService bettingService)
        {
            this.bettingService = bettingService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var bet = this.bettingService.Get(model => model.Status == BettingStatus.Continue)
                .OrderBy(model => model.FinishDate).FirstOrDefault();
            if (bet != null)
            {
                bet.Status = BettingStatus.Waiting;

                this.bettingService.Update(bet);
            }
            
            return Task.CompletedTask;
        }
    }
}
