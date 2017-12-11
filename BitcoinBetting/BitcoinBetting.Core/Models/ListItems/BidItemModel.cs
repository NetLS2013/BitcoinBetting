using System;

namespace BitcoinBetting.Core.Models.ListItems
{
    public class BidItemModel
    {
        public int BidId { get; set; }

        public int WalletId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentAddress { get; set; }

        public DateTime Date { get; set; }

        public decimal Coefficient { get; set; }

        public bool Status { get; set; }

        public bool Side { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string TransactionId { get; set; }

        public decimal PossibleWin { get; set; }
    }
    
    public enum PaymentStatus : byte
    {
        None,
        Unconfirmed, 
        Confirmed
    }
}