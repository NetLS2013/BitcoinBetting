using BitcoinBetting.Enum;

namespace BitcoinBetting.Core.Models
{
    public class Result
    {
        public int code { get; set; }

        public bool result { get; set; }

        public string token { get; set; }
        
        public string refresh_token { get; set; }

        public string Message {
            get
            {
                var result = ResponseMessage.Messages.TryGetValue((StatusMessage)code, out string message);
                return result ? message : "Internal application error";
            }
        }
    }
}
