using BitcoinBetting.Server.Models.Betting;
using BitcoinBetting.Server.Services.Contracts;
using System;
using System.Collections.Generic;

namespace BitcoinBetting.Server.Services.Betting
{
    public class WalletService : IWalletService
    {
        private IGenericRepository<WalletModel> repository;

        public WalletService(IGenericRepository<WalletModel> repository)
        {
            this.repository = repository;
        }

        public bool Create(WalletModel model)
        {
            try
            {
                this.repository.Create(model);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Remove(WalletModel model)
        {
            try
            {
                this.repository.Remove(model);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<WalletModel> Get(Func<WalletModel, bool> predicate)
        {
            try
            {
                var wallets = this.repository.Get(predicate);
                return wallets;
            }
            catch
            {
                return null;
            }
        }

        public WalletModel GetById(int id)
        {
            try
            {
                var wallet = this.repository.FindById(id);
                return wallet;
            }
            catch
            {
                return null;
            }
        }

    }
}
