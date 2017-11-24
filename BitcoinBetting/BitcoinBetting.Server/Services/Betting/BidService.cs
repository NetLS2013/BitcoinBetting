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
        private IGenericRepository<BettingModel> bettingRepository;

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
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Update(BidModel model)
        {
            try
            {
                this.repository.Update(model);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public IEnumerable<BidModel> Get(Func<BidModel, bool> predicate)
        {
            try
            {
                var bids = this.repository.Get(predicate);
                return bids;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public BidModel GetById(int id)
        {
            try
            {
                var bid = this.repository.FindById(id);
                return bid;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
