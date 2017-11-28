using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BitcoinBetting.Server.Services.Contracts;
using BitcoinBetting.Server.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace BitcoinBetting.Server.Services.Security
{
    public interface IJwtValidator
    {
        Task ValidateAsync(TokenValidatedContext context);
    }
    
    public class JwtValidator : IJwtValidator
    {
        private readonly IJwtToken jwtToken;
        
        public JwtValidator(IJwtToken jwtToken)
        {
            this.jwtToken = jwtToken;
        }
        
        public async Task ValidateAsync(TokenValidatedContext context)
        {
            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("This is not our issued token. It has no claims.");
                
                return;
            }
            
            var userId = claimsIdentity.FindFirst("ID").Value;
            var accessToken = context.SecurityToken as JwtSecurityToken;
            
            if (accessToken == null || string.IsNullOrWhiteSpace(accessToken.RawData) || !await jwtToken.IsValidTokenAsync(accessToken.RawData, userId))
            {
                context.Fail("This token is not in our database.");
                
                return;
            }
            
            //TODO update user last activity
        }
    }
}