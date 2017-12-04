namespace BitcoinBetting.UnitTest.ServicesTest
{
    using System;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using BitcoinBetting.Enum;
    using BitcoinBetting.Server.Controllers;
    using BitcoinBetting.Server.Database;
    using BitcoinBetting.Server.Database.Repositories;
    using BitcoinBetting.Server.Models.Account;
    using BitcoinBetting.Server.Models.Betting;
    using BitcoinBetting.Server.Services.Contracts;
    using BitcoinBetting.Server.Services.Identity;
    using BitcoinBetting.Server.Services.Security;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using NBitcoin;

    [TestClass]
    public class JwtTokenTest
    {
        private readonly IServiceProvider serviceProvider;

        private IJwtToken jwtToken;

        public JwtTokenTest()
        {
            var services = new ServiceCollection();
            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            services
                .AddDbContext<ApplicationDbContext>(b => b.UseInMemoryDatabase("bitcoinbetting").UseInternalServiceProvider(efServiceProvider));

            services.AddTransient<IGenericRepository<UserToken>, GenericRepository<UserToken>>();

            this.serviceProvider = services.BuildServiceProvider();
        }

        [TestInitialize]
        public async Task Initialize()
        {
           this.jwtToken = new JwtToken(this.serviceProvider.GetRequiredService<IGenericRepository<UserToken>>());
        }

        [TestMethod]
        public async Task Create_ValidParams_ExpectSuccess()
        {
            JwtSettings set = new JwtSettings() { SecretKey = "erobsj7VsrZ2byuIVV2vu1GbsICdPdrtLuJ85ZP", Audience = "audience", Issuer = "test"};

             var result = await this.jwtToken.CreateJwtTokens(set, new AppIdentityUser(){Email = "test@mail.com", Id = "TestId"}, String.Empty);

            Assert.IsNotNull(result.accessToken);
            Assert.IsNotNull(result.refreshToken);
        }
    }
}