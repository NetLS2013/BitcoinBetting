using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitcoinBetting.Server.Database;
using BitcoinBetting.Server.Services.Contracts;
using BitcoinBetting.Server.Services.Email;
using BitcoinBetting.Server.Services.Identity;
using BitcoinBetting.Server.Services.MailChimp;
using BitcoinBetting.Server.Services.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using BitcoinBetting.Server.Database.Repositories;
using BitcoinBetting.Server.Models.Betting;
using System.Timers;
using Hangfire;
using BitcoinBetting.Server.Services.Bitcoin;
using BitcoinBetting.Server.Services.Betting;

namespace BitcoinBetting.Server
{
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

            services.AddDbContext<ApplicationDbContext>(builder => builder.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));

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

            services.AddTransient<IBitcoinAverageApi, BitcoinAverageApi>(serviceProvider =>
            {
                return new BitcoinAverageApi(Configuration.GetSection("BitcoinAvarageSettings:PublicKey").Value, Configuration.GetSection("BitcoinAvarageSettings:SecretKey").Value);
            });

            services.AddTransient<IGenericRepository<BidModel>, GenericRepository<BidModel>>();
            services.AddTransient<IGenericRepository<WalletModel>, GenericRepository<WalletModel>>();
            services.AddTransient<IGenericRepository<BettingModel>, GenericRepository<BettingModel>>();

            services.AddTransient<IWalletService, WalletService>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();

//            app.UseHangfireServer();
//            app.UseHangfireDashboard();

           //RecurringJob.AddOrUpdate(() => Console.WriteLine("Minutely Job executed"), Cron.Minutely);
//            BackgroundJob.Schedule(() => Console.WriteLine("Minutely Job executed"), TimeSpan.FromDays(10));
        }
    }
}
