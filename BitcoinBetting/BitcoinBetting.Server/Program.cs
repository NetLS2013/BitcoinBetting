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
            var ads = new BitcoinWalletService(
                new BitcoinSettings()
                {
                    Password = "sdfdisghdsghiusdg",
                    Network = Network.TestNet,
                    Path =
                            "D:\\proj\\BitcoinBetting\\BitcoinBetting\\BitcoinBetting.Server\\wallet.dat"
                });
            foreach (var c in ads.GetAddresses(new List<int>() {0, 6, 7, 18 }))
            {
                Console.WriteLine(c.ToString());
            }
            Console.ReadKey();
            // BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .Build();
    }
}
