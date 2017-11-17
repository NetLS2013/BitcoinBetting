using BitcoinBetting.Server.Database.Helpers;
using BitcoinBetting.Server.Models.Betting;
using BitcoinBetting.Server.Services.Contracts;
using BitcoinBetting.Server.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class BidController : Controller
    {
        public IBidService bidService;
        public IBettingService bettingService;
        private readonly UserManager<AppIdentityUser> userManager;

        public BidController(
            IBidService bidService,
            IBettingService bettingService,
            UserManager<AppIdentityUser> userManager)
        {
            this.bidService = bidService;
            this.userManager = userManager;
            this.bettingService = bettingService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BidModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var bet = this.bettingService.GetById(model.BettingId);
            if (bet != null)
            {
                model.Date = DateTime.Now;
                model.UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
                model.Coefficient = BettingHelper.GetTimeCoeficient(bet.StartDate, bet.FinishDate, model.Date);

                var result = this.bidService.Create(model);

                if (result)
                {
                    return Ok(new { result = true });
                }
            }

            return Ok(new { result = false });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string userId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
            var bids = this.bidService.Get(x => x.UserId == userId);

            if (bids != null)
            {
                return Ok(new { result = true, list = bids });
            }

            return Ok(new { result = false });
        }

        [HttpGet]
        public async Task<IActionResult> Get(int bettingId)
        {
            var bids = this.bidService.Get(x => x.BettingId == bettingId);

            if (bids != null)
            {
                return Ok(new { result = true, list = bids });
            }

            return Ok(new { result = false });
        }

    }
}
