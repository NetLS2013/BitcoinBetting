using System.ComponentModel.DataAnnotations;

namespace BitcoinBetting.Server.Models.Account
{
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}