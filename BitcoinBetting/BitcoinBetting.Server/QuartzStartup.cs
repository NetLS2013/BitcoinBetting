namespace BitcoinBetting.Server
{
    using System;
    using System.Collections.Specialized;

    using BitcoinBetting.Server.Services.Betting;
    using BitcoinBetting.Server.Services.Betting.Jobs;

    using Quartz;
    using Quartz.Impl;

    public class QuartzStartup
    {
        private IScheduler scheduler;
        private readonly IServiceProvider container;

        public QuartzStartup(IServiceProvider container)
        {
            this.container = container;
        }

        public void Start()
        {
            if (this.scheduler != null)
            {
                throw new InvalidOperationException("Already started.");
            }

            var schedulerFactory = new StdSchedulerFactory();

            this.scheduler = schedulerFactory.GetScheduler().Result;
            this.scheduler.JobFactory = new JobFactory(this.container);
            this.scheduler.Start().Wait();

            var checkPayment = JobBuilder.Create<CheckPaymentJob>()
                .Build();

            var checkPaymentTrigger = TriggerBuilder.Create()
                .StartNow()
                .WithCronSchedule("0 0/5 * * * ?")
                .Build();

            this.scheduler.ScheduleJob(checkPayment, checkPaymentTrigger).Wait();

            var createBetting = JobBuilder.Create<CreateBettingJob>()
                .Build();

            var createBettingTrigger = TriggerBuilder.Create()
                .StartNow()
                .WithCronSchedule("0 0 0 1,10,20 * ?")
                .Build();

            this.scheduler.ScheduleJob(createBetting, createBettingTrigger).Wait();

            var setWait = JobBuilder.Create<SetWaitJob>()
                .Build();

            var setWaitTrigger = TriggerBuilder.Create()
                .StartNow()
                .WithCronSchedule("0 0 1 1,10,20 * ?")
                .Build();

            this.scheduler.ScheduleJob(setWait, setWaitTrigger).Wait();

            var award = JobBuilder.Create<AwardJob>()
                .Build();

            var awardTrigger = TriggerBuilder.Create()
                .StartNow()
                .WithCronSchedule("0 0 3 1,10,20 * ?")
                .Build();

            this.scheduler.ScheduleJob(award, awardTrigger).Wait();
        }

        public void Stop()
        {
            if (this.scheduler == null)
            {
                return;
            }

            if (this.scheduler.Shutdown(waitForJobsToComplete: true).Wait(30000))
            {
                this.scheduler = null;
            }
        }
    }
}