using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BitcoinBetting.Server.Database;
using BitcoinBetting.Server.Services.Contracts;
using BitcoinBetting.Server.Services.Email;
using BitcoinBetting.Server.Services.Identity;
using BitcoinBetting.Server.Services.MailChimp;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<AppIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddFacebook(f =>
                {
                    f.AppId = "158276314781134";
                    f.AppSecret = "6a46eb840dbe945a1c4717f3f79700b4";
                })
                .AddGoogle(g =>
                {
                    g.ClientId = "727512244362-2gm10t4ulfo72b79emnko9ikgf74lf46.apps.googleusercontent.com";
                    g.ClientSecret = "KvA6TwyqVFMgxxPdhDFyH09p";
                });
            
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IMailChimpSender, MailChimpSender>();
            
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
        }
    }
}
