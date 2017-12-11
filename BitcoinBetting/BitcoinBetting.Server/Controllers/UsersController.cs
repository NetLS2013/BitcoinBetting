using System.Linq;
using System.Threading.Tasks;
using BitcoinBetting.Server.Models.Users;
using BitcoinBetting.Server.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BitcoinBetting.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppIdentityUser> userManager;
        private readonly IHttpContextAccessor context;
        
        public UsersController(UserManager<AppIdentityUser> userManager, IHttpContextAccessor context)
        {
            this.userManager = userManager;
            this.context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUserData()
        {
            var user = await userManager.FindByNameAsync(context.HttpContext.User.Identity.Name);
            
            return Ok(new UserDataModel { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email });
        }
    }
}