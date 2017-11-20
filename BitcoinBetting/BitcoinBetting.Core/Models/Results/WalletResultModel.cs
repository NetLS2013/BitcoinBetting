using System.Collections.Generic;
using BitcoinBetting.Core.Models.ListItems;
using BitcoinBetting.Core.Models.Settings;

namespace BitcoinBetting.Core.Models.Results
{
    public class WalletResultModel : Result
    {
        public List<AddressItemModel> list { get; set; }
    }
}