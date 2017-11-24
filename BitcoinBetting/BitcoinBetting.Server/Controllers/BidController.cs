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
    using BitcoinBetting.Server.Models.Bitcoin;
    using BitcoinBetting.Server.Services.Bitcoin;

    [Authorize]
    [Route("api/[controller]/[action]")]
    public class BidController : Controller
    {
        private IBidService bidService;
        private IBettingService bettingService;

        private BitcoinWalletService bitcoinWalletService;

        private readonly UserManager<AppIdentityUser> userManager;

        public BidController(
            IBidService bidService,
            IBettingService bettingService,
            BitcoinWalletService bitcoinWalletService,
            UserManager<AppIdentityUser> userManager)
        {
            this.bidService = bidService;
            this.userManager = userManager;
            this.bettingService = bettingService;
            this.bitcoinWalletService = bitcoinWalletService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BidModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var bet = this.bettingService.GetById(model.BettingId);
            if (bet != null)
            {
                model.Date = DateTime.Now;
                model.UserId = (await this.userManager.FindByNameAsync(this.User.Identity.Name)).Id;
                model.Coefficient = BettingHelper.GetTimeCoefficient(bet.StartDate, bet.FinishDate, model.Date);
                model.PaymentStatus = PaymentStatus.None;
                

                var result = this.bidService.Create(model);
                model.PaymentAddress = this.bitcoinWalletService.GetAddressById(model.BidId).ToString();
                this.bidService.Update(model);

                if (result)
                {
                    return this.Ok(new { result = true, bid = model });
                }
            }

            return this.Ok(new { result = false });
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
        public async Task<IActionResult> GetById(int bettingId)
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
