using BitcoinBetting.Server.Models.Account;
using BitcoinBetting.Server.Models.Betting;
using BitcoinBetting.Server.Services.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BitcoinBetting.Server.Database
{
    public class ApplicationDbContext : IdentityDbContext<AppIdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            
        }
        
        public DbSet<WalletModel> Wallets { get; set; }

        public DbSet<BidModel> Bids { get; set; }

        public DbSet<BettingModel> Bettings { get; set; }
        
        public DbSet<UserToken> UserTokens { get; set; }
    }
}