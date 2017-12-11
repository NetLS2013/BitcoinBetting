using System.Threading.Tasks;
using BitcoinBetting.Server.Models.Account;
using BitcoinBetting.Server.Services.Identity;
using BitcoinBetting.Server.Services.Security;
using Microsoft.AspNetCore.Http;

namespace BitcoinBetting.Server.Services.Contracts
{
    public interface IJwtToken
    {
        Task<(string accessToken, string refreshToken)> CreateJwtTokens(JwtSettings jwtSettings, AppIdentityUser user, string deviceId);
        Task<UserToken> FindTokenAsync(string refreshToken, string deviceId);
        Task InvalidateTokensByDevice(string userId, string deviceId);
        Task<bool> IsValidTokenAsync(string accessToken, string userId);
        Task DeleteExpiredTokensAsync();
        string GetDeviceId(IHttpContextAccessor context);
        string GetSha256Hash(string input);
    }
}