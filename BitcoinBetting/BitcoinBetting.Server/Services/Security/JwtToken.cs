using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using BitcoinBetting.Server.Services.Contracts;
using BitcoinBetting.Server.Services.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BitcoinBetting.Server.Services.Security
{
    public class JwtToken : IJwtToken
    {
        public string GetAccessToken(AppIdentityUser user, JwtSettings jwtSettings)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var identity = new ClaimsIdentity(new GenericIdentity(user.Email), new[] { new Claim("ID", user.Id) });
            
            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: identity.Claims,
                expires: DateTime.UtcNow.AddMonths(1),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}