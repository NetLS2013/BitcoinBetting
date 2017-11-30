namespace BitcoinBetting.UnitTest
{
    using System;
    using System.Threading.Tasks;

    using BitcoinBetting.Server.Controllers;
    using BitcoinBetting.Server.Database;
    using BitcoinBetting.Server.Models.Account;
    using BitcoinBetting.Server.Services.Contracts;
    using BitcoinBetting.Server.Services.Identity;
    using BitcoinBetting.Server.Services.Security;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class AccountControllerTest
    {
        private readonly IServiceProvider serviceProvider;

        private AccountController accountController;

        public AccountControllerTest()
        {
            var services = new ServiceCollection();
            var efServiceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            services.AddOptions();

            services
                .AddDbContext<ApplicationDbContext>(b => b.UseInMemoryDatabase("bitcoinbetting").UseInternalServiceProvider(efServiceProvider));

            services.AddIdentity<AppIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            services.AddAuthentication(
                auth =>
                    {
                        auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    });

            services.AddMvc();

            var context = new DefaultHttpContext();
            services.AddSingleton<IHttpContextAccessor>(
                new HttpContextAccessor()
                    {
                        HttpContext = context,
                    });

            this.serviceProvider = services.BuildServiceProvider();
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

            var userManager = this.serviceProvider.GetRequiredService<UserManager<AppIdentityUser>>();
            await userManager.CreateAsync(user, "Pass@word1");

            var signInManager = this.serviceProvider.GetRequiredService<SignInManager<AppIdentityUser>>();

            // mock some services
            var emailMock = new Mock<IEmailSender>();
            var mailChimpMock = new Mock<IMailChimpSender>();
            var settingsMock = new Mock<IOptions<JwtSettings>>();
            var jwtMock = new Mock<IJwtToken>();
            var urlHelperMock = new Mock<IUrlHelper>();
            jwtMock.Setup(
                token => token.CreateJwtTokens(
                    settingsMock.Object.Value,
                    It.IsAny<AppIdentityUser>(),
                    It.IsAny<string>())).ReturnsAsync(() => ("Token", "Token"));
              //  .ReturnsAsync(() => Task.FromResult(Tuple<string, string>("UserToken", "UserToken")));

            // sign in test user
            var httpContext = this.serviceProvider.GetRequiredService<IHttpContextAccessor>();

            httpContext.HttpContext.RequestServices = this.serviceProvider;

            this.accountController = new AccountController(
                userManager,
                signInManager,
                emailMock.Object,
                mailChimpMock.Object,
                settingsMock.Object,
                jwtMock.Object,
                httpContext);
            this.accountController.ControllerContext.HttpContext = httpContext.HttpContext;
            this.accountController.Url = urlHelperMock.Object;
        }

        [TestMethod]
        public async Task Login_GivenExistUser_ExpectSuccess()
        {
            var result = await this.accountController.Login(
                             new LoginModel() { Email = "test@mail.com", Password = "Pass@word1" });

            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsNotNull(obj.token);
        }

        [TestMethod]
        public async Task Login_GivenNotExistUser_ExpectError()
        {
            var result = await this.accountController.Login(
                             new LoginModel() { Email = "test@mail.com", Password = "somepass" });

            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsNotNull(obj.code);
        }

        [TestMethod]
        public async Task Register_GivenValidUserModel_ExpectSuccess()
        {
            var result = await this.accountController.Register(
                             new RegisterModel()
                                 {
                                     Email = "register@mail.com",
                                     Password = "pAss@word1",
                                     FirstName = "asd",
                                     LastName = "asd"
                                 });

            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsNotNull(obj.token);
        }

        [TestMethod]
        public async Task Register_GivenValidUserModelButExistedEmail_ExpectError()
        {
            var result = await this.accountController.Register(
                             new RegisterModel() { Email = "test@mail.com", Password = "pAss@word1" });

            var okResult = result as OkObjectResult;

            dynamic obj = new DynamicObjectResultValue(okResult.Value);

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsNotNull(obj.code);
        }
    }
}