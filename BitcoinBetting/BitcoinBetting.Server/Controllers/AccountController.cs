using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using BitcoinBetting.Enum;
using BitcoinBetting.Server.Models.Account;
using BitcoinBetting.Server.Services.Contracts;
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
        private readonly IMailChimpSender mailChimp;

        public AccountController(
            UserManager<AppIdentityUser> userManager,
            SignInManager<AppIdentityUser> signInManager,
            IEmailSender emailSender,
            IMailChimpSender mailChimp)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.mailChimp = mailChimp;
        }
        
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

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
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (await userManager.FindByEmailAsync(model.Email) != null)
            {
                return Ok(new { code = StatusMessage.EmailDuplicate, result = false });
            }

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
                var callbackurl = Url.Action(nameof(ConfirmEmail), "Account",new { userId = user.Id, code = confrimationCode }, Request.Scheme);

                try
                {
                    await emailSender.SendEmailAsync(user.Email, "BitcoinBetting support" ,"Confirm email link:\n" + callbackurl);
                    await mailChimp.AddUserAsync(user.Email, user.FirstName, user.LastName);
                }
                catch (Exception e)
                {
                    return Ok(new { code = StatusMessage.ErrorCreatingUser, result = false });
                }
                
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

        [HttpGet]
        public async Task<IActionResult> ExternalLogin()
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), new { returnUrl = "/" });
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return Challenge(properties, "Google");
        }
        
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
            
            if (result.Succeeded)
            {
                return Ok(new { result = true });
            }
            
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                
            return Ok(new { result = true, email, provider = info.LoginProvider });
        }
        
        public async Task<IActionResult> ExternalLoginConfirmation([FromBody] RegisterModel model)
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            
            var user = new AppIdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            
            var result = await userManager.CreateAsync(user);
            
            if (result.Succeeded)
            {
                result = await userManager.AddLoginAsync(user, info);
                
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    
                    return Ok(new { result = true });
                }
            }

            return null;
        }
    }
}
