using BitcoinBetting.Server.Models.Betting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Services.Contracts
{
    public interface IWalletService
    {
        IEnumerable<WalletModel> Get(Func<WalletModel, bool> predicate);

        WalletModel GetById(int id);

        bool Create(WalletModel model);

        bool Remove(WalletModel model);
    }
}
