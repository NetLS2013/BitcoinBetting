namespace BitcoinBetting.Server.Models.Betting
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using BitcoinBetting.Enum;
    using BitcoinBetting.Server.Helpers;

    [Table("Bettings")]
    public class BettingModel
    {
        [Key]
        public int BettingId { get; set; }

        public decimal ExchangeRate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public BettingStatus Status { get; set; }

        public bool? Result { get; set; }

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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            BettingModel other = obj as BettingModel;
            if ((Object)other == null)
                return false;

            return this.BettingId == other.BettingId
                   && this.ExchangeRate == other.ExchangeRate
                   && this.StartDate == other.StartDate
                   && this.FinishDate == other.FinishDate
                   && this.Status == other.Status
                   && this.Result == other.Result;
        }
    }
}
