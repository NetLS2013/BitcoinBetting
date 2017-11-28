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
using BitcoinBetting.Server.Models.Account;
using BitcoinBetting.Server.Models.Betting;
using BitcoinBetting.Server.Services.Bitcoin;
using BitcoinBetting.Server.Services.Betting;

namespace BitcoinBetting.Server
{
    using BitcoinBetting.Server.Models;
    using BitcoinBetting.Server.Services.Betting.Jobs;

    using NBitcoin;

    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtSettings>(Configuration.GetSection("JWTSettings"));
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<AppIdentityUser, IdentityRole>(option =>
            {
                option.Password = new PasswordOptions
                {
                    RequireNonAlphanumeric = false,
                    RequireUppercase = false,
                    RequireDigit = false
                };
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            
            //var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            //optionsBuilder.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));

            //services.AddSingleton<ApplicationContext>(builder => new ApplicationContext(optionsBuilder.Options));
            //services.AddDbContext<ApplicationContext>(builder => builder.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

            services
                .AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration.GetSection("JWTSettings:Issuer").Value,
                        ValidAudience = Configuration.GetSection("JWTSettings:Audience").Value,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.ASCII.GetBytes(Configuration.GetSection("JWTSettings:SecretKey").Value)),

                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true
                    };

                    bearer.RequireHttpsMetadata = false;

                    bearer.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<IJwtValidator>();
                            
                            return tokenValidatorService.ValidateAsync(context);
                        }
                    };
                })
                .AddFacebook(facebook =>
                {
                    facebook.AppId = "158276314781134";
                    facebook.AppSecret = "6a46eb840dbe945a1c4717f3f79700b4";
                })
                .AddGoogle(google =>
                {
                    google.ClientId = "727512244362-2gm10t4ulfo72b79emnko9ikgf74lf46.apps.googleusercontent.com";
                    google.ClientSecret = "KvA6TwyqVFMgxxPdhDFyH09p";
                });
            
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IMailChimpSender, MailChimpSender>();
            services.AddTransient<IJwtToken, JwtToken>();
            
            services.AddTransient<IJwtValidator, JwtValidator>();

            services.AddTransient<IBitcoinAverageApi, BitcoinAverageApi>(serviceProvider =>
            {
                return new BitcoinAverageApi(Configuration.GetSection("BitcoinAvarageSettings:PublicKey").Value, Configuration.GetSection("BitcoinAvarageSettings:SecretKey").Value);
            });

            services.Configure<BitcoinSettings>(settings => this.Configuration.GetSection("BitcoinSettings"));
            
            services.AddTransient<IGenericRepository<BidModel>, GenericRepository<BidModel>>();
            services.AddTransient<IGenericRepository<WalletModel>, GenericRepository<WalletModel>>();
            services.AddTransient<IGenericRepository<BettingModel>, GenericRepository<BettingModel>>();
            services.AddTransient<IGenericRepository<UserToken>, GenericRepository<UserToken>>();
            
            services.AddTransient<IBettingService, BettingService>();
            services.AddTransient<IBidService, BidService>();
            
            services.AddTransient<IWalletService, WalletService>();
            services.AddTransient<IBettingService, BettingService>();
            services.AddTransient<IBidService, BidService>();

           // services.AddTransient<BitcoinWalletService>();//(provider => new BitcoinWalletService(new BitcoinSettings() { Password = "sdfdisghdsghiusdg", Network = Network.TestNet, Path = "D:\\proj\\BitcoinBetting\\BitcoinBetting\\BitcoinBetting.Server\\wallet.dat" }));
           services.AddTransient<BitcoinWalletService>(provider => new BitcoinWalletService(new BitcoinSettings() { Password = "sdfdisghdsghiusdg", Network = Network.TestNet, Path = "/Users/keiqsa/Projects/BitcoinBetting/BitcoinBetting/BitcoinBetting.Server/wallet.dat" }));

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

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
