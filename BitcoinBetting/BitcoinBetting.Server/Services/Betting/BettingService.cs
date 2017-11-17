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
        TimeSpan time;

        public BettingService(IGenericRepository<BettingModel> repository, IBitcoinAverageApi bitcoinAverage, TimeSpan time)
        {
            this.repository = repository;
            this.bitcoinAverage = bitcoinAverage;
            this.time = time;
        }

        public IEnumerable<BettingModel> Get()
        {
            try
            {
                var bettings = this.repository.Get();

                foreach (var bet in bettings)
                {
                    bet.Bank = bidRepository.Get(x => x.BettingId == bet.BettingId).Sum<BidModel>(x => x.Amount);
                }

                return bettings;
            }
            catch
            {
                return null;
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
            catch
            {
                return null;
            }
        }

        public async Task<bool> Create()
        {
            var betting = new BettingModel();
            betting.StartDate = DateTime.Now;
            betting.FinishDate = DateTime.Now.Add(time);

            var excangeRate = (await bitcoinAverage.GetShortDataAsync())?["BTCUSD"]?["averages"]?["day"]?.ToObject<double>();
            if (!excangeRate.HasValue)
            {
                return false;
            }

            betting.ExchangeRate = excangeRate.Value;

            try
            {
                this.repository.Create(betting);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public BettingModel GetById(int id)
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
