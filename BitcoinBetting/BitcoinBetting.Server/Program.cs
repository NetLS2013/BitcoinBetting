using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BitcoinBetting.Server
{
    using System;
    using System.Collections.Generic;

    using BitcoinBetting.Server.Models;
    using BitcoinBetting.Server.Services.Bitcoin;

    using NBitcoin;

    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .Build();
    }
}
