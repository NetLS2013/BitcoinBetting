using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using BitcoinBetting.Enum;
using BitcoinBetting.Server.Models;
using BitcoinBetting.Server.Models.Account;
using BitcoinBetting.Server.Services.Contracts;
using BitcoinBetting.Server.Services.Email;
using BitcoinBetting.Server.Services.Identity;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BitcoinBetting.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppIdentityUser> userManager;
        private readonly SignInManager<AppIdentityUser> signInManager;
        private readonly IEmailSender emailSender;
        
        public AccountController(
            UserManager<AppIdentityUser> userManager,
            SignInManager<AppIdentityUser> signInManager,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var passwordSignIn = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                
                if (passwordSignIn.Succeeded)
                {
                    return Ok(new { result = true });
                }
            }
            
            return Ok(new { code = StatusMessage.UsernameOrPasswordIncorrect, result = false });
        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return StatusCode(StatusCodes.Status204NoContent);
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = new AppIdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var create = await userManager.CreateAsync(user, model.Password);
            
            if (create.Succeeded)
            {
                var confrimationCode = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackurl = Url.Action("ConfirmEmail", new { userId = user.Id, code = confrimationCode });

                await emailSender.SendEmailAsync(user.Email, "Confirm Email", callbackurl);
                
                var passwordSignIn = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    
                if (passwordSignIn.Succeeded)
                {
                    return Ok(new { result = true });
                }
            }
            
            return Ok(new { code = StatusMessage.ErrorCreatingUser, result = false });
        }
        
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }
            
            var user = await userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var confirmEmail = await userManager.ConfirmEmailAsync(user, code);
                
                if (confirmEmail.Succeeded)
                {
                    return Ok("Your email was sent successfully");
                }
            }
            
            return BadRequest();
        }
    }
}
