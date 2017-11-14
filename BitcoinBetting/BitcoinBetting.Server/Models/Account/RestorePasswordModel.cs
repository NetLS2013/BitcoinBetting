using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Models.Account
{
    public class RestorePasswordModel
    {
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
