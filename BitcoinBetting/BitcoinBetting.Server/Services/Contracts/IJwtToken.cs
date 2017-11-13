using BitcoinBetting.Server.Services.Identity;
using BitcoinBetting.Server.Services.Security;

namespace BitcoinBetting.Server.Services.Contracts
{
    public interface IJwtToken
    {
        string GetAccessToken(AppIdentityUser user, JwtSettings jwtSettings);
    }
}