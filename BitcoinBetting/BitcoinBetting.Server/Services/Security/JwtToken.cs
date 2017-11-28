using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using BitcoinBetting.Server.Models.Account;
using BitcoinBetting.Server.Services.Contracts;
using BitcoinBetting.Server.Services.Identity;

using Microsoft.IdentityModel.Tokens;

namespace BitcoinBetting.Server.Services.Security
{
    public class JwtToken : IJwtToken
    {
        private readonly IGenericRepository<UserToken> repository;
        
        public JwtToken(IGenericRepository<UserToken> repository)
        {
            this.repository = repository;
        }
        
        public async Task<(string accessToken, string refreshToken)> CreateJwtTokens(JwtSettings jwtSettings, AppIdentityUser user)
        {
            var now = DateTimeOffset.UtcNow;
            var accessTokenExpiresDateTime = now.AddMinutes(10);
            var refreshTokenExpiresDateTime = now.AddMinutes(60);
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var identity = new ClaimsIdentity(new GenericIdentity(user.Email), new[] { new Claim("ID", user.Id) });
            
            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: identity.Claims,
                expires: accessTokenExpiresDateTime.UtcDateTime,
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = Guid.NewGuid().ToString().Replace("-", "");
            
            await AddUserTokenAsync(user, refreshToken, accessToken, refreshTokenExpiresDateTime, accessTokenExpiresDateTime);
            
            return (accessToken, refreshToken);
        }

        private async Task AddUserTokenAsync(AppIdentityUser user, string refreshToken, string accessToken,
            DateTimeOffset refreshTokenExpiresDateTime, DateTimeOffset accessTokenExpiresDateTime)
        {
            var token = new UserToken
            {
                UserId = user.Id,
                RefreshTokenIdHash = GetSha256Hash(refreshToken),
                AccessTokenHash = GetSha256Hash(accessToken),
                RefreshTokenExpiresDateTime = refreshTokenExpiresDateTime,
                AccessTokenExpiresDateTime = accessTokenExpiresDateTime
            };
            
            repository.Create(token);
        }
        
        public async Task<UserToken> FindTokenAsync(string refreshToken)
        {
            return await repository.FindAsync(x => x.RefreshTokenIdHash == GetSha256Hash(refreshToken));
        }
        
        public async Task InvalidateUserTokensAsync(string accessToken)
        {
            var userTokens = await repository.FindAsync(x => x.AccessTokenHash == GetSha256Hash(accessToken));
            
            repository.Remove(userTokens);
        }
        
        public async Task<bool> IsValidTokenAsync(string accessToken, string userId)
        {
            var userToken = await repository.FindAsync(x => x.AccessTokenHash == GetSha256Hash(accessToken) && x.UserId == userId);
            
            return userToken?.AccessTokenExpiresDateTime >= DateTime.UtcNow;
        }
        
        public string GetSha256Hash(string input)
        {
            using (var hashAlgorithm = new SHA256CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                
                return Convert.ToBase64String(byteHash);
            }
        }
    }
}