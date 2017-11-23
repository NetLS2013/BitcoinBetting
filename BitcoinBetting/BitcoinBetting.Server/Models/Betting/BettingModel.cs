using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Models.Betting
{
    using System.ComponentModel.DataAnnotations;

    using BitcoinBetting.Server.Database.Helpers;

    [Table("Bettings")]
    public class BettingModel
    {
        [Key]
        public int BettingId { get; set; }

        public decimal ExchangeRate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public BettingStatus Status { get; set; }

       // public BettingStatus BettingStatus { get; set; }

        [NotMapped]
        public decimal Bank { get; set; }

        [NotMapped]
        public decimal BankLess { get; set; }

        [NotMapped]
        public decimal BankMore { get; set; }

        [NotMapped]
        public decimal CoefficientMore => BettingHelper.GetCoefficient(StartDate, FinishDate, BankMore, BankLess);

        [NotMapped]
        public decimal CoefficientLess => BettingHelper.GetCoefficient(StartDate, FinishDate, BankLess, BankMore);
    }
}
