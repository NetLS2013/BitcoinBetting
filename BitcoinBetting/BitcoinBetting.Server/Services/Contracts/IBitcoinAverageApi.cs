using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Services.Contracts
{
    public interface IBitcoinAverageApi
    {
        Task<JToken> GetJsonAsync(string url);

        Task<JToken> GetAllTimeHistoricalDataAsync(string symbol = "BTCUSD", string market = "global");

        Task<JToken> GetDailyDataAsync(string symbol = "BTCUSD", string market = "global");

        Task<JToken> GetShortDataAsync(string crypto = "BTC", string fiat = "USD", string market = "global");

        Task<JToken> GetOhlcAsync(string symbol = "BTCUSD", string market = "global");

    }
}
