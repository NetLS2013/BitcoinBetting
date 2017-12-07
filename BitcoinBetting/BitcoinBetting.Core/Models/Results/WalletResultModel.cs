using System.Collections.Generic;
using BitcoinBetting.Core.Models.ListItems;

namespace BitcoinBetting.Core.Models.Results
{
    public class WalletResultModel : ResultModel
    {
        public List<AddressItemModel> List { get; set; }
    }
}