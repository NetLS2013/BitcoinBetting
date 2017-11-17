using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBetting.Server.Models.Betting
{
    [Table("Bettings")]
    public class BettingModel
    {
        public int BettingId { get; set; }

        public double ExchangeRate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        [NotMapped]
        public double Bank { get; set; }

    }
}
