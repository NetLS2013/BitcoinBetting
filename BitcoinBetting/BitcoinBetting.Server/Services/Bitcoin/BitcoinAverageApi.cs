using BitcoinBetting.Server.Services.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Services.Bitcoin
{
    public class BitcoinAverageApi : IBitcoinAverageApi
    {
        private readonly string publicKey;
        private readonly string secretKey;
        private readonly HMACSHA256 sigHasher;
        private readonly DateTime epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public BitcoinAverageApi(string publicKey, string secretKey)
        {
            this.secretKey = secretKey;
            this.publicKey = publicKey;
            sigHasher = new HMACSHA256(Encoding.ASCII.GetBytes(this.secretKey));
        }

        public async Task<JToken> GetJsonAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-signature", GetHeaderSignature());
                return JToken.Parse(await httpClient.GetStringAsync(url));
            }
        }

        public Task<JToken> GetAllTimeHistoricalDataAsync(string symbol = "BTCUSD", string market = "global")
        {
            var url = "https://apiv2.bitcoinaverage.com/indices/" + market + "/history/" + symbol + "?"
                   + "&period=alltime"
                   + "&format=json";

            return GetJsonAsync(url);
        }

        public Task<JToken> GetDailyDataAsync(string symbol = "BTCUSD", string market = "global")
        {
            var url = "https://apiv2.bitcoinaverage.com/indices/" + market + "/history/" + symbol + "?"
                   + "&period=daily"
                   + "&format=json";

            return GetJsonAsync(url);
        }

        public Task<JToken> GetShortDataAsync(string crypto = "BTC", string fiat = "USD", string market = "global")
        {
            var url = "https://apiv2.bitcoinaverage.com/indices/" + market + "/ticker/short?crypto=" + crypto
                   + "&fiat=" + fiat;

            return GetJsonAsync(url);
        }
        
        public Task<JToken> GetOhlcAsync(string symbol = "BTCUSD", string market = "global")
        {
            var url = "https://apiv2.bitcoinaverage.com/indices/" + market + "/ticker/" + symbol;

            return GetJsonAsync(url);
        }

        private string GetHeaderSignature()
        {
            var timestamp = (int)((DateTime.UtcNow - epochUtc).TotalSeconds);
            var payload = timestamp + "." + publicKey;
            var digestValueBytes = sigHasher.ComputeHash(Encoding.ASCII.GetBytes(payload));
            var digestValueHex = BitConverter.ToString(digestValueBytes).Replace("-", "").ToLower();
            return payload + "." + digestValueHex;
        }
    }
}
