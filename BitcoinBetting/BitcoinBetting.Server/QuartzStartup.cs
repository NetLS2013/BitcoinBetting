namespace BitcoinBetting.Server
{
    using System;
    using System.Collections.Specialized;

    using BitcoinBetting.Server.Services.Betting;

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

            var userEmailsJob = JobBuilder.Create<CreateBettingJob>()
                .Build();

            var userEmailsTrigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(builder => builder.WithIntervalInSeconds(1).RepeatForever())
                .Build();

            this.scheduler.ScheduleJob(userEmailsJob, userEmailsTrigger).Wait();
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