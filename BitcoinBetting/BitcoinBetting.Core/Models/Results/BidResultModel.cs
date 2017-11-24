using System.Collections.Generic;
using BitcoinBetting.Core.Models.Betting;
using BitcoinBetting.Core.Models.ListItems;

namespace BitcoinBetting.Core.Models.Results
{
    public class BidResultModel : Result
    {
        public BidModel bid { get; set; }
        public List<BidItemModel> list { get; set; }
    }
}