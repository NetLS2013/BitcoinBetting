using BitcoinBetting.Server.Models.Betting;
using BitcoinBetting.Server.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Services.Betting
{
    public class BettingService : IBettingService
    {
        IGenericRepository<BettingModel> repository;
        IGenericRepository<BidModel> bidRepository;
        IBitcoinAverageApi bitcoinAverage;

        public BettingService(IGenericRepository<BettingModel> repository, IBitcoinAverageApi bitcoinAverage, IGenericRepository<BidModel> bidRepository)
        {
            this.repository = repository;
            this.bitcoinAverage = bitcoinAverage;
            this.bidRepository = bidRepository;
        }

        public IEnumerable<BettingModel> Get()
        {
            try
            {
                var bettings = this.repository.Get();

                foreach (var bet in bettings)
                {
                    bet.Bank = this.GetBank(bet.BettingId);
                    bet.BankLess = this.GetBank(bet.BettingId, false);
                    bet.BankMore = this.GetBank(bet.BettingId, true);
                }

                return bettings;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public decimal? CurrentExchange => bitcoinAverage.GetShortDataAsync().Result?["BTCUSD"]?["averages"]?["day"]
            ?.ToObject<decimal>();

        public decimal GetBank(int betId, bool? side = null)
        {
            if (side.HasValue)
            {
                return this.bidRepository.Get(model => model.BettingId == betId && model.Side == side).Sum(model => model.Amount);
            }

            return this.bidRepository.Get(model => model.BettingId == betId).Sum(model => model.Amount);
        }

        public async Task<bool> Update(BettingModel betting)
        {
            try
            {
                this.repository.Update(betting);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public IEnumerable<BettingModel> Get(Func<BettingModel, bool> predicate)
        {
            try
            {
                var bettings = this.repository.Get(predicate);

                foreach(var bet in bettings)
                {
                    bet.Bank = bidRepository.Get(x => x.BettingId == bet.BettingId).Sum<BidModel>(x => x.Amount);
                }

                return bettings;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<bool> Create(BettingModel betting)
        {
            try
            {
                this.repository.Create(betting);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public BettingModel GetById(int id)
        {
            try
            {
                var bet = this.repository.FindById(id);
                bet.Bank = this.GetBank(bet.BettingId);
                bet.BankLess = this.GetBank(bet.BettingId, false);
                bet.BankMore = this.GetBank(bet.BettingId, true);

                return bet;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
