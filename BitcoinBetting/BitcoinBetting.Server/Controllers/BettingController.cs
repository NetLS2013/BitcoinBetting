using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BitcoinBetting.Server.Services.Identity;
using BitcoinBetting.Server.Services.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace BitcoinBetting.Server.Controllers
{
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
            var bids = this.bettingService.Get();

            if (bids != null)
            {
                return Ok(new { result = true, list = bids });
            }

            return Ok(new { result = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int bettingId)
        {
            var bid = this.bettingService.GetById(bettingId);

            if (bid != null)
            {
                return Ok(new { result = true, list = bid });
            }

            return Ok(new { result = false });
        }


    }
}
