using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Models.Account
{
    public class ForgotPasswordConfirmModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
