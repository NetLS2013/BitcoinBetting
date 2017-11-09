using BitcoinBetting.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinBetting.Core.Models.User
{
    public class Result
    {
        public int code { get; set; }

        public bool result { get; set; }

        public string Message {
            get
            {
                var result = ResponseMessage.Messages.TryGetValue((StatusMessage)code, out string message);
                return result ? message : "Internal application error";
            }
        }
    }
}
