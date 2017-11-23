using System;

namespace BitcoinBetting.Server.Database.Helpers
{
    public class BettingHelper
    {
        public static decimal GetTimeCoefficient(DateTime start, DateTime finish, DateTime currDate)
        {
            var totalSeconds = (decimal)(finish - start).TotalSeconds;
            var totalSecondsToCurr = (decimal)(finish - currDate).TotalSeconds;

            return ((totalSeconds - totalSecondsToCurr) / totalSeconds) + 1;
        }

        public static decimal GetAmountPayment(decimal bet, decimal coefficient, decimal betBank, decimal oppositeBank)
        {
            return bet + (((bet * coefficient) / (betBank * coefficient)) * oppositeBank);
        }

        public static decimal GetCoefficient(DateTime start, DateTime finish, decimal betBank, decimal oppositeBank)
        {
            decimal bet = 1M;

            decimal possibleWin = GetAmountPayment(
                bet,
                GetTimeCoefficient(start, finish, DateTime.Now),
                betBank,
                oppositeBank);

            return possibleWin;
        }
    }
}
