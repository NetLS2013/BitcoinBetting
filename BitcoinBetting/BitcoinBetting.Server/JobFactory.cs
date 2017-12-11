namespace BitcoinBetting.Server
{
    using System;

    using Quartz;
    using Quartz.Spi;
    public class JobFactory : IJobFactory
    {
        protected readonly IServiceProvider Container;

        public JobFactory(IServiceProvider container)
        {
            this.Container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var c = this.Container.GetService(bundle.JobDetail.JobType) as IJob;
            return c;
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}