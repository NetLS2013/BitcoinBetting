using System.ComponentModel.DataAnnotations;

namespace BitcoinBetting.Server.Models.Account
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}