using System.Collections.Generic;
using BitcoinBetting.Core.Models.ListItems;

namespace BitcoinBetting.Core.Models.Results
{
    public class BettingResultModel : ResultModel
    {
        public List<BettingItemModel> list { get; set; }
    }
}