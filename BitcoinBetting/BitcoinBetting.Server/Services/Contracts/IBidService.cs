using BitcoinBetting.Server.Models.Betting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Services.Contracts
{
    public interface IBidService
    {
        bool Create(BidModel model);

        bool Update(BidModel model);

        IEnumerable<BidModel> Get(Func<BidModel, bool> predicate);

        BidModel GetById(int id);
    }
}
