namespace BitcoinBetting.Server.Helpers
{
    using System;

    public class BettingHelper
    {
        private static decimal percent = 0.9M;

        public static decimal GetTimeCoefficient(DateTime start, DateTime finish, DateTime currDate)
        {
            if (start > finish || currDate > finish || currDate < start)
            {
                return 1;
            }

            var totalSeconds = (decimal)(finish - start).TotalSeconds;
            var totalSecondsToCurr = (decimal)(currDate - start).TotalSeconds;

            return ((totalSeconds - totalSecondsToCurr) / totalSeconds) + 1;
        }

        public static decimal GetAmountPayment(decimal bet, decimal coefficient, decimal betBank, decimal oppositeBank)
        {
            if (bet < 0 || coefficient < 0 || betBank < 0 || oppositeBank < 0)
            {
                throw new ArgumentException();
            }

            if (betBank == 0)
            {
                return bet;
            }

            return bet + (((bet * coefficient * percent) / (betBank * coefficient)) * oppositeBank);
        }

        public static decimal GetCoefficient(DateTime start, DateTime finish, decimal betBank, decimal oppositeBank)
        {
            decimal bet = 1M;

            if (betBank == 0)
            {
                betBank = bet;
            }

            decimal possibleWin = GetAmountPayment(
                bet,
                GetTimeCoefficient(start, finish, DateTime.Now),
                betBank,
                oppositeBank);

            return possibleWin;
        }
    }
}
