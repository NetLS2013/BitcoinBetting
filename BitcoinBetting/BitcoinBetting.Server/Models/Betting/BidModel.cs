using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Models.Betting
{
    using System.ComponentModel.DataAnnotations;

    using BitcoinBetting.Server.Models.Bitcoin;

    [Table("Bids")]
    public class BidModel
    {
        [Key]
        public int BidId { get; set; }

        public int BettingId { get; set; }

        public string UserId { get; set; }

        public int WalletId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentAddress { get; set; }

        public DateTime Date { get; set; }

        public decimal Coefficient { get; set; }

        public bool Status { get; set; }

        public bool Side { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public bool Paid { get; set; }

        [NotMapped]
        public decimal PossibleWin { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            BidModel other = obj as BidModel;
            if ((Object)other == null)
                return false;

            return this.BidId == other.BidId && this.BettingId == other.BettingId && this.UserId == other.UserId
                   && this.WalletId == other.WalletId && this.Amount == other.Amount
                   && this.PaymentAddress == other.PaymentAddress && this.Date == other.Date
                   && this.Coefficient == other.Coefficient && this.Status == other.Status && this.Side == other.Side
                   && this.PaymentStatus == other.PaymentStatus && this.Paid == other.Paid;
        }
    }
}
