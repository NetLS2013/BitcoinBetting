namespace BitcoinBetting.UnitTest.ControllersTest
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using BitcoinBetting.Enum;
    using BitcoinBetting.Server.Controllers;
    using BitcoinBetting.Server.Database;
    using BitcoinBetting.Server.Database.Repositories;
    using BitcoinBetting.Server.Models;
    using BitcoinBetting.Server.Models.Betting;
    using BitcoinBetting.Server.Services.Betting;
    using BitcoinBetting.Server.Services.Bitcoin;
    using BitcoinBetting.Server.Services.Contracts;
    using BitcoinBetting.Server.Services.Identity;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BettingControllerTest
    {
        private readonly IServiceProvider _serviceProvider;

        private BettingController bettingController;

        private ICollection bettingModels;

        public BettingControllerTest()
        {
            var services = new ServiceCollection();
            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            services.AddOptions();

            services.AddDbContext<ApplicationDbContext>(
                b => b.UseInMemoryDatabase("bitcoinbetting").UseInternalServiceProvider(efServiceProvider));

            services.AddIdentity<AppIdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IBitcoinAverageApi, BitcoinAverageApi>(
                provider => new BitcoinAverageApi(
                    new OptionsWrapper<BitcoinAvarageSettings>(
                        new BitcoinAvarageSettings() { SecretKey = "", PublicKey = "" })));

            services.AddTransient<IGenericRepository<BidModel>, GenericRepository<BidModel>>();
            services.AddTransient<IGenericRepository<BettingModel>, GenericRepository<BettingModel>>();
            services.AddTransient<IGenericRepository<BettingModel>, GenericRepository<BettingModel>>();

            services.AddTransient<IBidService, BidService>();
            services.AddTransient<IBettingService, BettingService>();

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

            this.bettingController = new BettingController(
                this._serviceProvider.GetRequiredService<IBidService>(),
                this._serviceProvider.GetRequiredService<IBettingService>(),
                userManager);

            this.bettingController.ControllerContext.HttpContext = httpContext;

            var bettingCurrent = new BettingModel()
                                     {
                                         Status = BettingStatus.Continue,
                                         BettingId = 1,
                                         ExchangeRate = 1000,
                                         StartDate = DateTime.Now,
                                         FinishDate = DateTime.Now.AddDays(1)
                                     };
            var bettingDone = new BettingModel()
                                  {
                                      Status = BettingStatus.Done,
                                      BettingId = 2,
                                      ExchangeRate = 1000,
                                      StartDate = DateTime.Now,
                                      FinishDate = DateTime.Now.AddDays(1)
                                  };

            this.bettingModels = new List<BettingModel>() { bettingCurrent, bettingDone };

            var bettingService = this._serviceProvider.GetRequiredService<IGenericRepository<BettingModel>>();
            bettingService.Create(bettingCurrent);
            bettingService.Create(bettingDone);
        }

        [TestMethod]
        public async Task Get_NoParams_ExpectSuccess()
        {
            var result = await this.bettingController.Get();
            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            CollectionAssert.AreEqual(obj.list, this.bettingModels);
        }

        [TestMethod]
        public async Task GetById_ValidParams_ExpectSuccess()
        {
            var result = await this.bettingController.GetById(1);
            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(obj.list, ((List<BettingModel>)this.bettingModels)[0]);
        }

        [TestMethod]
        public async Task GetById_InvalidParams_ExpectError()
        {
            var result = await this.bettingController.GetById(4);
            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsFalse(obj.result);
        }

        [TestMethod]
        public async Task GetCurrent_NoParams_ExpectSuccess()
        {
            var result = await this.bettingController.GetCurrent();
            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsTrue(obj.result);
        }

        [TestMethod]
        public async Task GetArchive_NoParams_ExpectSuccess()
        {
            var result = await this.bettingController.GetArchive();
            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsTrue(obj.result);
        }
    }
}