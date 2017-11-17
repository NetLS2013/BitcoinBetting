using BitcoinBetting.Server.Models.Betting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Database.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {

        }

        public DbSet<WalletModel> Wallets { get; set; }

        public DbSet<BidModel> Bids { get; set; }

        public DbSet<BettingModel> Bettings { get; set; }
    }
}
