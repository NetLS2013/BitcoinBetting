namespace BitcoinBetting.Server.Controllers
{
    using System.Threading.Tasks;

    using BitcoinBetting.Server.Models.Betting;
    using BitcoinBetting.Server.Services.Contracts;
    using BitcoinBetting.Server.Services.Identity;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [Route("api/[controller]/[action]")]
    public class BettingController : Controller
    {
        private readonly UserManager<AppIdentityUser> userManager;
        public IBidService bidService;
        public IBettingService bettingService;

        public BettingController(
            IBidService bidService,
            IBettingService bettingService,
            UserManager<AppIdentityUser> userManager)
        {
            this.bidService = bidService;
            this.userManager = userManager;
            this.bettingService = bettingService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var bettings = this.bettingService.Get();

            if (bettings != null)
            {
                return this.Ok(new { result = true, list = bettings });
            }

            return this.Ok(new { result = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int bettingId)
        {
            var betting = this.bettingService.GetById(bettingId);

            if (betting != null)
            {
                return this.Ok(new { result = true, list = betting });
            }

            return this.Ok(new { result = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrent()
        {
            var bettings = this.bettingService.Get(model => model.Status != BettingStatus.Done);

            if (bettings != null)
            {
                return this.Ok(new { result = true, list = bettings });
            }

            return this.Ok(new { result = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetArchive()
        {
            var bettings = this.bettingService.Get(model => model.Status == BettingStatus.Done);

            if (bettings != null)
            {
                return this.Ok(new { result = true, list = bettings });
            }

            return this.Ok(new { result = false });
        }
    }
}
