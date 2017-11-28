using System.Threading.Tasks;
using BitcoinBetting.Server.Models.Account;
using BitcoinBetting.Server.Services.Identity;
using BitcoinBetting.Server.Services.Security;

namespace BitcoinBetting.Server.Services.Contracts
{
    public interface IJwtToken
    {
        Task<(string accessToken, string refreshToken)> CreateJwtTokens(JwtSettings jwtSettings, AppIdentityUser user);
        Task<UserToken> FindTokenAsync(string refreshToken);
        Task InvalidateUserTokensAsync(string userId);
        Task<bool> IsValidTokenAsync(string accessToken, string userId);
    }
}