using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Models.Betting
{
    [Table("Wallets")]
    public class WalletModel
    {
        [Key]
        public int WalletId { get; set; }

        public string UserId { get; set; }

        [Required]
        public string Address { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            WalletModel other = obj as WalletModel;
            if ((Object)other == null)
                return false;

            return this.WalletId == other.WalletId
                   && this.Address == other.Address
                   && this.UserId == other.UserId;
        }
    }
}
