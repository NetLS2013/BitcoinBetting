using System;
using System.Text;

using BitcoinBetting.Server.Database;
using BitcoinBetting.Server.Services.Contracts;
using BitcoinBetting.Server.Services.Email;
using BitcoinBetting.Server.Services.Identity;
using BitcoinBetting.Server.Services.MailChimp;
using BitcoinBetting.Server.Services.Security;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using BitcoinBetting.Server.Database.Repositories;
using BitcoinBetting.Server.Models.Betting;
using BitcoinBetting.Server.Services.Bitcoin;
using BitcoinBetting.Server.Services.Betting;

namespace BitcoinBetting.Server
{
    using System.IO;

    using BitcoinBetting.Server.Database.Context;
    using BitcoinBetting.Server.Models;
    using BitcoinBetting.Server.Services.Betting.Jobs;

    using NBitcoin;

    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<JwtSettings>(this.Configuration.GetSection("JWTSettings"));

            services.Configure<BitcoinAvarageSettings>(this.Configuration.GetSection("BitcoinAvarageSettings"));

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ApplicationContext>(
                builder => builder.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<AppIdentityUser, IdentityRole>(
                option =>
                    {
                        option.Password = new PasswordOptions
                                              {
                                                  RequireNonAlphanumeric = false,
                                                  RequireUppercase = false,
                                                  RequireDigit = false
                                              };
                    }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication(
                auth =>
                    {
                        auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }).AddJwtBearer(
                bearer =>
                    {
                        bearer.TokenValidationParameters = new TokenValidationParameters
                                                               {
                                                                   ValidIssuer =
                                                                       this.Configuration
                                                                           .GetSection(
                                                                               "JWTSettings:Issuer")
                                                                           .Value,
                                                                   ValidAudience =
                                                                       this.Configuration
                                                                           .GetSection(
                                                                               "JWTSettings:Audience")
                                                                           .Value,
                                                                   IssuerSigningKey =
                                                                       new
                                                                           SymmetricSecurityKey(
                                                                               Encoding.ASCII
                                                                                   .GetBytes(
                                                                                       this
                                                                                           .Configuration
                                                                                           .GetSection(
                                                                                               "JWTSettings:SecretKey")
                                                                                           .Value)),
                                                                   ValidateIssuer = true,
                                                                   ValidateAudience = true,
                                                                   ValidateIssuerSigningKey =
                                                                       true,
                                                                   ValidateLifetime = false
                                                               };

                        bearer.RequireHttpsMetadata = false;
                    }).AddFacebook(
                facebook =>
                    {
                        facebook.AppId = "158276314781134";
                        facebook.AppSecret = "6a46eb840dbe945a1c4717f3f79700b4";
                    }).AddGoogle(
                google =>
                    {
                        google.ClientId = "727512244362-2gm10t4ulfo72b79emnko9ikgf74lf46.apps.googleusercontent.com";
                        google.ClientSecret = "KvA6TwyqVFMgxxPdhDFyH09p";
                    });

            services.AddTransient<IBitcoinAverageApi, BitcoinAverageApi>();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IMailChimpSender, MailChimpSender>();
            services.AddTransient<IJwtToken, JwtToken>();

            services.AddTransient<IGenericRepository<BidModel>, GenericRepository<BidModel>>();
            services.AddTransient<IGenericRepository<WalletModel>, GenericRepository<WalletModel>>();
            services.AddTransient<IGenericRepository<BettingModel>, GenericRepository<BettingModel>>();

            services.AddTransient<IWalletService, WalletService>();
            services.AddTransient<IBettingService, BettingService>();
            services.AddTransient<IBidService, BidService>();

            services.AddTransient<IBitcoinWalletService, BitcoinWalletService>(
                provider => new BitcoinWalletService(
                    new BitcoinSettings()
                        {
                            Password =
                                this.Configuration.GetSection("BitcoinSettings:Password").Value,
                            Path = this.Configuration.GetSection("BitcoinSettings:Path").Value,
                            NetworkStr =
                                this.Configuration.GetSection("BitcoinSettings:NetworkStr").Value
                        }));       

            services.AddTransient<CreateBettingJob>();
            services.AddTransient<SetWaitJob>();
            services.AddTransient<CheckPaymentJob>();
            services.AddTransient<AwardJob>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IServiceProvider container)
        {
            var quartz = new QuartzStartup(container);

            lifetime.ApplicationStarted.Register(quartz.Start);
            lifetime.ApplicationStopping.Register(quartz.Stop);

            if (!File.Exists(this.Configuration.GetSection("BitcoinSettings:Path").Value))
            {
                container.GetService<IBitcoinWalletService>().GenerateWallet();
            }
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
