using System;
using BitcoinBetting.Enum;

namespace BitcoinBetting.Core.Models.ListItems
{
    public class BettingItemModel
    {
        public int BettingId { get; set; }
        public double ExchangeRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public double Bank { get; set; }
        public bool Side { get; set; }
        public decimal BankMore { get; set; }
        public decimal BankLess { get; set; }
        public BettingStatus Status { get; set; }
        
        public string Address { get; set; }
        public int WalletId { get; set; }
        public string TimeLeft {
            get
            {
                var timeSpan = FinishDate - DateTime.Now;

                return timeSpan.ToString("d'd 'm'm 's's'");
            }
        }
        
        public decimal CoefficientMore { get; set; }
        public decimal CoefficientLess { get; set; }
        
        public bool? Result { get; set; }
    }
}