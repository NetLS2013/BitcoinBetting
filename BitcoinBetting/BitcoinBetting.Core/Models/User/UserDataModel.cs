using System;

namespace BitcoinBetting.Core.Models.User
{
    public class UserDataModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        
        public string UserName => String.Concat(FirstName, " ", LastName);
    }
}