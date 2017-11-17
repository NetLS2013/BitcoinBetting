using BitcoinBetting.Server.Models.Betting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Services.Contracts
{
    public interface IBettingService
    {
        Task<bool> Create();

        IEnumerable<BettingModel> Get(Func<BettingModel, bool> predicate);

        IEnumerable<BettingModel> Get();

        BettingModel GetById(int id);
    }
}
