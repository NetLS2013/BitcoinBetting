using System;

namespace BitcoinBetting.Server.Database.Helpers
{
    public class BettingHelper
    {
        public static double GetTimeCoeficient(DateTime start, DateTime finish, DateTime currDate)
        {
            var totalSeconds = (finish - start).TotalSeconds;
            var totalSecondsToCurr = (finish - currDate).TotalSeconds;

            return ((totalSeconds - totalSecondsToCurr) / totalSeconds) + 1;
        }

        public static double GetAmountPayment(double bet, double coeficient, double betBank, double oppositeBank)
        {
            return bet + (((bet * coeficient) / (betBank * coeficient)) * oppositeBank);
        }
    }
}
