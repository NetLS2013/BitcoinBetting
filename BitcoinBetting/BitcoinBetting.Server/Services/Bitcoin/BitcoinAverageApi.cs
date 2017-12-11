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
    using BitcoinBetting.Server.Models;

    using Microsoft.Extensions.Options;

    public class BitcoinAverageApi : IBitcoinAverageApi
    {
        private readonly string publicKey;
        private readonly string secretKey;
        private readonly HMACSHA256 sigHasher;
        private readonly DateTime epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public BitcoinAverageApi(IOptions<BitcoinAvarageSettings> options)
        {
            secretKey = options.Value.SecretKey;
            publicKey = options.Value.PublicKey;
            this.sigHasher = new HMACSHA256(Encoding.ASCII.GetBytes(this.secretKey));
        }

        public async Task<JToken> GetJsonAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-signature", this.GetHeaderSignature());

                return JToken.Parse(await httpClient.GetStringAsync(url));
            }
        }

        public Task<JToken> GetAllTimeHistoricalDataAsync(string symbol = "BTCUSD", string market = "global")
        {
            var url = "https://apiv2.bitcoinaverage.com/indices/" + market + "/history/" + symbol + "?"
                   + "&period=alltime"
                   + "&format=json";

            return this.GetJsonAsync(url);
        }

        public Task<JToken> GetDailyDataAsync(string symbol = "BTCUSD", string market = "global")
        {
            var url = "https://apiv2.bitcoinaverage.com/indices/" + market + "/history/" + symbol + "?"
                   + "&period=daily"
                   + "&format=json";

            return this.GetJsonAsync(url);
        }

        public Task<JToken> GetShortDataAsync(string crypto = "BTC", string fiat = "USD", string market = "global")
        {
            var url = "https://apiv2.bitcoinaverage.com/indices/" + market + "/ticker/short?crypto=" + crypto
                   + "&fiat=" + fiat;

            return this.GetJsonAsync(url);
        }
        
        public Task<JToken> GetOhlcAsync(string symbol = "BTCUSD", string market = "global")
        {
            var url = "https://apiv2.bitcoinaverage.com/indices/" + market + "/ticker/" + symbol;

            return this.GetJsonAsync(url);
        }

        private string GetHeaderSignature()
        {
            var timestamp = (int)(DateTime.UtcNow - this.epochUtc).TotalSeconds;
            var payload = timestamp + "." + this.publicKey;
            var digestValueBytes = this.sigHasher.ComputeHash(Encoding.ASCII.GetBytes(payload));
            var digestValueHex = BitConverter.ToString(digestValueBytes).Replace("-", string.Empty).ToLower();

            return payload + "." + digestValueHex;
        }
    }
}
