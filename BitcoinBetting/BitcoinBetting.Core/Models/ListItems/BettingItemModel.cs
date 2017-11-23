﻿using System;
using BitcoinBetting.Core.ViewModels;

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
    }
    
    public enum BettingStatus : byte
    {
        Waiting,
        Done
    }
}