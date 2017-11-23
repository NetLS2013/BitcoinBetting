using System;
using BitcoinBetting.Core.ViewModels;

namespace BitcoinBetting.Core.Models.ListItems
{
    public class BettingItemModel
    {
        public double ExchangeRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public double Bank { get; set; }
        public bool BetType { get; set; }
        public string Address { get; set; }
    }
}