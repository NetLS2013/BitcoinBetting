using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BitcoinBetting.Server.Database;
using BitcoinBetting.Server.Services.Contracts;
using BitcoinBetting.Server.Services.Email;
using BitcoinBetting.Server.Services.Identity;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

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

            services.AddIdentity<AppIdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = "/Account/Login";
            //    options.LogoutPath = "/Account/Logout";
            //    options.SlidingExpiration = true;
            //    options.Cookie = new CookieBuilder
            //    {
            //        HttpOnly = true,
            //        Name = ".Fiver.Security.Cookie",
            //        Path = "/",
            //        SameSite = SameSiteMode.Lax,
            //        SecurePolicy = CookieSecurePolicy.SameAsRequest
            //    };
            //});

            services.AddTransient<IEmailSender, EmailSender>();
            
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
