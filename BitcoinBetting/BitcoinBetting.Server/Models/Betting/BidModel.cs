using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Models.Betting
{
    [Table("Bids")]
    public class BidModel
    {
        [Key]
        public int BidId { get; set; }

        public int BettingId { get; set; }

        public string UserId { get; set; }

        public int WalletId { get; set; }

        public double Amount { get; set; }

        public string PaymentAddress { get; set; }

        public DateTime Date { get; set; }

        public double Coefficient { get; set; }

        public bool Status { get; set; }

        public bool Side { get; set; }
    }
}
