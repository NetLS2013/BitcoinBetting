using BitcoinBetting.Enum;
using Newtonsoft.Json;

namespace BitcoinBetting.Core.Models
{
    public class ResultModel
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        
        public string Message {
            get
            {
                var result = ResponseMessage.Messages.TryGetValue((StatusMessage)Code, out string message);
                
                return result ? message : "Internal application error";
            }
        }
    }
}
