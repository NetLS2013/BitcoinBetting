using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Models.Bitcoin
{
    using NBitcoin;

    public class TransactionHistoryRecord
    {
        public DateTime Date { get; set; }

        public Money Money { get; set; }

        public bool IsConfirmed { get; set; }

        public uint256 TransactionId { get; set; }
    }
}
