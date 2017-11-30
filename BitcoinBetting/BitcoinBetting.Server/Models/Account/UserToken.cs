using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BitcoinBetting.Server.Services.Identity;

namespace BitcoinBetting.Server.Models.Account
{
    [Table("UserToken")]
    public class UserToken
    {
        [Key]
        public int Id { get; set; }

        public string AccessTokenHash { get; set; }
        public DateTimeOffset AccessTokenExpiresDateTime { get; set; }

        public string RefreshTokenIdHash { get; set; }
        public DateTimeOffset RefreshTokenExpiresDateTime { get; set; }

        public string UserId { get; set; }
        public string DeviceId { get; set; }
    }
}