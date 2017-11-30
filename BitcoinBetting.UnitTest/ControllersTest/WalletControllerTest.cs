namespace BitcoinBetting.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using BitcoinBetting.Server.Controllers;
    using BitcoinBetting.Server.Database;
    using BitcoinBetting.Server.Database.Repositories;
    using BitcoinBetting.Server.Models.Betting;
    using BitcoinBetting.Server.Services.Betting;
    using BitcoinBetting.Server.Services.Contracts;
    using BitcoinBetting.Server.Services.Identity;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WalletControllerTest
    {
        private readonly IServiceProvider _serviceProvider;

        private WalletController walletController;

        public WalletControllerTest()
        {
            var services = new ServiceCollection();
            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            services.AddOptions();

            services.AddDbContext<ApplicationDbContext>(
                b => b.UseInMemoryDatabase("bitcoinbetting").UseInternalServiceProvider(efServiceProvider));

            services.AddIdentity<AppIdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IGenericRepository<WalletModel>, GenericRepository<WalletModel>>();

            services.AddTransient<IWalletService, WalletService>();

            services.AddMvc();

            var context = new DefaultHttpContext();
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor() { HttpContext = context, });

            this._serviceProvider = services.BuildServiceProvider();
        }

        [TestInitialize]
        public async Task Initialize()
        {
            // create test user
            var user = new AppIdentityUser
                           {
                               Id = "TestUser",
                               UserName = "test@mail.com",
                               Email = "test@mail.com",
                               TwoFactorEnabled = true
                           };

            var userManager = this._serviceProvider.GetRequiredService<UserManager<AppIdentityUser>>();
            await userManager.CreateAsync(user, "Pass@word1");

            // sign in test user
            var httpContext = this._serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(new GenericIdentity(user.Email), new[] { new Claim("ID", user.Id) }));
            httpContext.RequestServices = this._serviceProvider;

            this.walletController = new WalletController(
                this._serviceProvider.GetRequiredService<IWalletService>(),
                userManager);

            this.walletController.ControllerContext.HttpContext = httpContext;
        }

        [TestMethod]
        public async Task CreateWalletAddress_GivenValidModel_ExpectSuccess()
        {
            var result = await this.walletController.Create(new WalletModel() { Address = "WalletAddress" });
            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsTrue(obj.result);
        }

        [TestMethod]
        public async Task RemoveWalletAddress_GivenValidId_ExpectSuccess()
        {
            await this.walletController.Create(new WalletModel() { Address = "WalletAddress", WalletId = 100 });
            var result = await this.walletController.Remove(100);
            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsTrue(obj.result);
        }

        [TestMethod]
        public async Task RemoveWalletAddress_GivenValidInvalidId_ExpectSuccess()
        {
            var result = await this.walletController.Remove(100);
            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsFalse(obj.result);
        }

        [TestMethod]
        public async Task GetWalletAddresses_NoParams_ExpectSuccess()
        {
            // add wallets
            var repository = this._serviceProvider.GetRequiredService<IGenericRepository<WalletModel>>();
            var listWallets = new List<WalletModel>()
                                  {
                                      new WalletModel() { UserId = "TestUser", Address = "TestAddress1" },
                                      new WalletModel() { UserId = "TestUser", Address = "TestAddress2" },
                                      new WalletModel() { UserId = "TestUser", Address = "TestAddress3" }
                                  };

            repository.Create(listWallets[0]);
            repository.Create(listWallets[1]);
            repository.Create(listWallets[2]);
            repository.Create(new WalletModel() { UserId = "SomeOtherUser", Address = "TestAddress3" });

            var result = await this.walletController.Get();
            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            CollectionAssert.AreEqual(obj.list, listWallets);
        }
    }
}