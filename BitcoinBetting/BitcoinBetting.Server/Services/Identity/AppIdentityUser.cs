using Microsoft.AspNetCore.Identity;

namespace BitcoinBetting.Server.Services.Identity
{
    public class AppIdentityUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RestorePassCode { get; set; }
    }
}