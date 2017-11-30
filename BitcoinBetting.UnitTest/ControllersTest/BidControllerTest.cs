namespace BitcoinBetting.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    using BitcoinBetting.Enum;
    using BitcoinBetting.Server.Controllers;
    using BitcoinBetting.Server.Database;
    using BitcoinBetting.Server.Database.Context;
    using BitcoinBetting.Server.Database.Repositories;
    using BitcoinBetting.Server.Models;
    using BitcoinBetting.Server.Models.Betting;
    using BitcoinBetting.Server.Models.Bitcoin;
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

    using Moq;

    using NBitcoin;

    [TestClass]
    public class BidControllerTest
    {
        private readonly IServiceProvider _serviceProvider;

        private BidController bidController;

        public BidControllerTest()
        {
            var services = new ServiceCollection();
            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            services.AddOptions();

            services.AddDbContext<ApplicationDbContext>(
                b => b.UseInMemoryDatabase("bitcoinbetting").UseInternalServiceProvider(efServiceProvider));

            services.AddDbContext<ApplicationContext>(
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
                                      Status = BettingStatus.Waiting,
                                      BettingId = 2,
                                      ExchangeRate = 1000,
                                      StartDate = DateTime.Now,
                                      FinishDate = DateTime.Now.AddDays(1)
                                  };
            var bettingService = this._serviceProvider.GetRequiredService<IGenericRepository<BettingModel>>();
            bettingService.Create(bettingCurrent);
            bettingService.Create(bettingDone);

            var urlHelperMock = new Mock<IBitcoinWalletService>();
            urlHelperMock.Setup(service => service.GetAddressById(It.IsAny<int>())).Returns(
                () => BitcoinAddress.Create("mhheFUrieWV2zVsdWXNZkqSmeSVjkbXWer", Network.TestNet));
            this.bidController = new BidController(
                this._serviceProvider.GetRequiredService<IBidService>(),
                this._serviceProvider.GetRequiredService<IBettingService>(),
                urlHelperMock.Object,
                userManager);

            // sign in test user
            var httpContext = this._serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(new GenericIdentity(user.Email), new[] { new Claim("ID", user.Id) }));
            httpContext.RequestServices = this._serviceProvider;
            this.bidController.ControllerContext.HttpContext = httpContext;
        }

        [TestMethod]
        public async Task Create_ValidParams_ExpectSuccess()
        {
            var result = await this.bidController.Create(new BidModel() { WalletId = 1, BettingId = 1, Side = true });

            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsTrue(obj.result);

            Assert.AreEqual(obj.bid.UserId, "TestUser");
            Assert.AreEqual(obj.bid.WalletId, 1);
            Assert.AreEqual(obj.bid.Amount, 0M);
            Assert.AreEqual(obj.bid.PaymentAddress, "mhheFUrieWV2zVsdWXNZkqSmeSVjkbXWer");
            Assert.IsTrue(obj.bid.Side);
            Assert.AreEqual(obj.bid.PaymentStatus, PaymentStatus.None);
            Assert.IsFalse(obj.bid.Paid);
        }

        [TestMethod]
        public async Task Create_InvalidParamsBetNotAllow_ExpectError()
        {
            var result = await this.bidController.Create(new BidModel() { WalletId = 1, BettingId = 2, Side = true });

            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsFalse(obj.result);
        }

        [TestMethod]
        public async Task Create_InvalidBetId_ExpectError()
        {
            var result = await this.bidController.Create(new BidModel() { WalletId = 1, BettingId = 3, Side = true });

            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsFalse(obj.result);
        }

        [TestMethod]
        public async Task GetById_ValidParams_ExpectSuccess()
        {
            var bidService = this._serviceProvider.GetRequiredService<IGenericRepository<BidModel>>();
            var bid = new BidModel() { BettingId = 2, Amount = 1M, Side = false, BidId = 8, Coefficient = 2, Date = DateTime.Now, UserId = "TestUser", WalletId = 1, Paid = false, PaymentStatus = PaymentStatus.Confirmed, Status = false, PaymentAddress = "mhheFUrieWV2zVsdWXNZkqSmeSVjkbXWer" };
            var bidList = new List<BidModel>(){ bid };
            bidService.Create(bidList[0]);

            var result = await this.bidController.GetById(2);

            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            CollectionAssert.AreEqual(obj.list, bidList);
        }

        [TestMethod]
        public async Task GetById_ValidParams_ExpectSuccessEmptyList()
        {

            var result = await this.bidController.GetById(4);

            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsTrue(obj.result);
            CollectionAssert.AreEqual(obj.list, new List<BidModel>());
        }
    }
}