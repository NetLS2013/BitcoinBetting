using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinBetting.Core.Models.User
{
    public class ExternalLoginConfirmModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string Cookie { get; set; }

        [JsonIgnore]
        public string Provider { get; set; }
    }
}
