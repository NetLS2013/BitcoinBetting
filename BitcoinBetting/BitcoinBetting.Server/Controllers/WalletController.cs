using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BitcoinBetting.Server.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using BitcoinBetting.Server.Models.Betting;
using BitcoinBetting.Server.Services.Identity;
using Microsoft.AspNetCore.Identity;

namespace BitcoinBetting.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class WalletController : Controller
    {
        public IWalletService walletService;
        private readonly UserManager<AppIdentityUser> userManager;

        public WalletController(
            IWalletService walletService,
            UserManager<AppIdentityUser> userManager)
        {
            this.walletService = walletService;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string userId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
            var vallets = this.walletService.Get(x => x.UserId == userId);

            if (vallets != null)
            {
                return Ok(new { result = true, list = vallets });
            }

            return Ok(new { result = false });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WalletModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            model.UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
            var result = this.walletService.Create(model);

            if (result)
            {
                return Ok(new { result = true });
            }

            return Ok(new { result = false });
        }

        [HttpDelete]
        public async Task<IActionResult> Remove([FromBody] int id)
        {
            var model = this.walletService.GetById(id);
            string userId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;

            if (model != null && model.UserId == userId)
            {
                if (this.walletService.Remove(model))
                {
                    return Ok(new { result = true });
                }
                else
                {
                    return Ok(new { result = false });
                }
            }

            return Ok(new { result = false });
        }

    }
}