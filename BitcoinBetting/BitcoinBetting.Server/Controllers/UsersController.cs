using System.Linq;

using BitcoinBetting.Server.Services.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BitcoinBetting.Server.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<AppIdentityUser> userManager;

        public UsersController(UserManager<AppIdentityUser> userManager)
        {
            this.userManager = userManager;
        }
    }
}