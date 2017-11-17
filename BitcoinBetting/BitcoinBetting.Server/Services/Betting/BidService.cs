using BitcoinBetting.Server.Database.Helpers;
using BitcoinBetting.Server.Models.Betting;
using BitcoinBetting.Server.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Services.Betting
{
    public class BidService : IBidService
    {
        private IGenericRepository<BidModel> repository;

        public BidService(IGenericRepository<BidModel> repository)
        {
            this.repository = repository;
        }

        public bool Create(BidModel model)
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

        public IEnumerable<BidModel> Get(Func<BidModel, bool> predicate)
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

        public BidModel GetById(int id)
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
