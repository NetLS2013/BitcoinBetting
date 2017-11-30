namespace BitcoinBetting.UnitTest.HelpersTest
{
    using System;

    using BitcoinBetting.Server.Helpers;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BettingHelperTests
    {
        [TestMethod]
        public void GetTimeCoefficient_ValidParams_ExpectCoeffOneAndHalf()
        {
            var offset = TimeSpan.FromSeconds(1000);
            var start = DateTime.Now.Date;
            var finish = DateTime.Now.Date.Add(offset);
            var curr = DateTime.Now.Date.Add(offset / 2);

            var coefficient = BettingHelper.GetTimeCoefficient(start, finish, curr);

            Assert.AreEqual(coefficient, 1.5M);
        }

        [TestMethod]
        public void GetTimeCoefficient_ValidParams_ExpectCoeffOne()
        {
            var offset = TimeSpan.FromSeconds(1000);
            var start = DateTime.Now.Date;
            var finish = DateTime.Now.Date.Add(offset);
            var curr = finish;

            var coefficient = BettingHelper.GetTimeCoefficient(start, finish, curr);

            Assert.AreEqual(coefficient, 1M);
        }

        [TestMethod]
        public void GetTimeCoefficient_ValidParams_ExpectCoeffTwo()
        {
            var offset = TimeSpan.FromSeconds(1000);
            var start = DateTime.Now.Date;
            var finish = DateTime.Now.Date.Add(offset);
            var curr = start;

            var coefficient = BettingHelper.GetTimeCoefficient(start, finish, curr);

            Assert.AreEqual(coefficient, 2M);
        }

        [TestMethod]
        public void GetTimeCoefficient_InvalidParams_ExpectException()
        {
            var start = DateTime.MaxValue;
            var finish = DateTime.Now.Date;
            var curr = DateTime.MinValue;

            Assert.ThrowsException<ArgumentException>(() => BettingHelper.GetTimeCoefficient(start, finish, curr));
        }

        [TestMethod]
        public void GetAmountPayment_ValidParams_ExpectBetReturn()
        {
            var bet = 1M;
            var coefficient = 1M;
            var betBank = 0M;
            var oppositeBank = 0M;

            var amount = BettingHelper.GetAmountPayment(bet, coefficient, betBank, oppositeBank);

            Assert.AreEqual(amount, bet);
        }

        [TestMethod]
        public void GetAmountPayment_ValidParams_ExpectWin()
        {
            var bet = 1M;
            var coefficient = 2M;
            var betBank = 1M;
            var oppositeBank = 1M;

            var amount = BettingHelper.GetAmountPayment(bet, coefficient, betBank, oppositeBank);

            Assert.AreEqual(amount, 1.9M);
        }

        [TestMethod]
        public void GetAmountPayment_InvalidParams_ExpectException()
        {
            var bet = -1M;
            var coefficient = 0M;
            var betBank = 0M;
            var oppositeBank = 0M;

            Assert.ThrowsException<ArgumentException>(() => BettingHelper.GetAmountPayment(bet, coefficient, betBank, oppositeBank));
        }
    }
}